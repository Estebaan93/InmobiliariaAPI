//Controllers/InmueblesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using InmobiliariaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaAPI.Models;
using InmobiliariaAPI.Models.ViewModels;
using InmobiliariaAPI.Data;
using System.Text.Json;
using System.ComponentModel.DataAnnotations; // Para ValidationContext

namespace InmobiliariaAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class InmueblesController : ControllerBase
  {
    private readonly IInmuebleRepositorio _repo;
    private readonly IWebHostEnvironment _env;
    private readonly InmobiliariaContext _context;  //inyecta el context
    public InmueblesController(IInmuebleRepositorio repo, IWebHostEnvironment env, InmobiliariaContext context)
    {
      _repo = repo;
      _env = env;
      _context = context;
    }

    //GET: api/Inmuebles/obtenerInmueble (obtiene inmuebles del propietario)
    [HttpGet("obtener")]
    public IActionResult ObtenerPorPropietario()
    {
      //Obtenemos el id del propietario logueado desde el token
      var idPropietario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

      var inmuebles = _repo.ObtenerTodosPorPropietario(idPropietario);
      if (inmuebles == null || inmuebles.Count == 0)
      {
        return NotFound("No se encontraron inmuebles para este propietario");

      }
      return Ok(inmuebles);
    }

    //GET: api/Inmuebles/activos que estan alquilados
    [HttpGet("activos")]
    public IActionResult ObtenerActivosPorPropietario()
    {
      var idPropietario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var inmuebles = _repo.ObtenerActivosPorPropietario(idPropietario);

      if (inmuebles == null || inmuebles.Count == 0)
      {
        return NotFound("No se encontraron inmuebles activos para este propietario");
      }

      return Ok(inmuebles);
    }


    //POST: api/Inmuebles/nuevo cargar inmueble + Json {inmueble} + img por separado
    /*Validar el propietario logueado y que el inmueble a agregar le pertenezca, ej puede otro propietario con su token agregar inmuebles a su nombre
    ej si la inmobiliaria solo deja agregar 3 inmuebles por propietario...*/
    [HttpPost("nuevo")]
    [RequestSizeLimit(10_000_000)] //Hasta 10 MB
    [Consumes("multipart/form-data")]
    public IActionResult CrearNuevoInmueble([FromForm] InmuebleCargaDTO carga)
    {
      // Validar que se envio la imagen
      if (carga.Imagen == null || carga.Imagen.Length == 0)
        return BadRequest("La imagen es requerida");

      // Validar que se envio el JSON del inmueble
      if (string.IsNullOrWhiteSpace(carga.Inmueble))
        return BadRequest("Los datos del inmueble son requeridos");

      // Deserializar el JSON a InmuebleJsonDTO
      InmuebleJsonDTO dto;
      try
      {
        dto = JsonSerializer.Deserialize<InmuebleJsonDTO>(carga.Inmueble, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        }) ?? throw new JsonException("El JSON deserializado es nulo.");
      }
      catch (JsonException ex)
      {
        return BadRequest($"Error al procesar el JSON: {ex.Message}");
      }

      // Validar el modelo manualmente (ya que viene como string)

      // Usamos DataAnnotations manualmente ya que el DTO viene como string
      var validationContext = new ValidationContext(dto, null, null);
      var validationResults = new List<ValidationResult>();
      //True me valida todas las propiedades, si fuera False solo me valida las Requerid
      bool isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true); 

      if (!isValid)
      {
        // Devuelve el primer error de validacion encontrado
        return BadRequest(validationResults.First().ErrorMessage);
      }

      // Iniciar transaccion atomica
      using (var transaction = _context.Database.BeginTransaction())
      {
        try
        {
          var idPropietario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

          //Logica de "Buscar o Crear" Direccion
          string calleTrim = dto.Calle.Trim();
          string cpTrim = dto.Cp.Trim();
          string ciudadTrim = dto.Ciudad.Trim();

          //Busca si existe una direccion igual
          var direccionExistente = _context.Direcciones.FirstOrDefault(d =>
              d.Calle == calleTrim &&
              d.Altura == dto.Altura &&
              d.Cp == cpTrim &&
              d.Ciudad == ciudadTrim
          );

          Direccion direccionParaInmueble;
          if (direccionExistente != null)
          {
            direccionParaInmueble = direccionExistente;
          }
          else
          {
            var nuevaDireccion = new Direccion
            {
              Calle = calleTrim,
              Altura = dto.Altura,
              Cp = cpTrim,
              Ciudad = ciudadTrim,
              Coordenadas = dto.Coordenadas.Trim()
            };
            _context.Direcciones.Add(nuevaDireccion);
            // Guardamos aqui para obtener el IdDireccion
            _context.SaveChanges();
            direccionParaInmueble = nuevaDireccion;
          }

          //Chequeo de Duplicados (ahora con el IdDireccion correcto)
          var inmuebleDuplicado = _context.Inmuebles.FirstOrDefault(i =>
              i.IdPropietario == idPropietario &&
              i.IdDireccion == direccionParaInmueble.IdDireccion && // <-- Id ya est disponible
              i.IdTipo == dto.IdTipo &&
              i.Metros2 == dto.Metros2 &&
              i.CantidadAmbientes == dto.CantidadAmbientes
          );

          if (inmuebleDuplicado != null)
          {
            transaction.Rollback(); // Deshace el SaveChanges de la direccion (si era nueva)
            return Conflict(new
            {
              mensaje = "Ya existe un inmueble con caracteristicas identicas.",
              idInmuebleExistente = inmuebleDuplicado.IdInmueble
            });
          }

          //Logica de Guardado de Archivo
          string carpeta = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "imagenes_inmuebles");
          if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

          // Nombre de archivo seguro: GUID + extension
          string extension = Path.GetExtension(carga.Imagen.FileName);
          string nombreArchivo = $"{Guid.NewGuid()}{extension}";
          string rutaArchivo = Path.Combine(carpeta, nombreArchivo);

          using (var stream = new FileStream(rutaArchivo, FileMode.Create))
          {
            carga.Imagen.CopyTo(stream);
          }

          string urlImagen = $"{Request.Scheme}://{Request.Host}/imagenes_inmuebles/{nombreArchivo}";

          //Creacion del Inmueble
          var inmuebleNuevo = new Inmueble
          {
            IdPropietario = idPropietario,
            IdDireccion = direccionParaInmueble.IdDireccion,
            IdTipo = dto.IdTipo,
            Metros2 = dto.Metros2,
            CantidadAmbientes = dto.CantidadAmbientes,
            Precio = dto.Precio,
            Descripcion = dto.Descripcion.Trim(),
            Cochera = dto.Cochera,
            Piscina = dto.Piscina,
            Mascotas = dto.Mascotas,
            UrlImagen = urlImagen,
            Estado = false // Pendiente de habilitacion
          };

          //Usar el Repositorio (solo agrega al contexto)
          var creado = _repo.CrearInmueble(inmuebleNuevo);

          //Guardado Final
          _context.SaveChanges(); // Guarda el inmueble nuevo

          //Confirmar Transaccion
          transaction.Commit();

          //Cargar datos relacionados para la respuesta
          creado.Tipo = _context.Tipos.Find(creado.IdTipo);
          creado.Direccion = direccionParaInmueble;

          //Respuesta Exitosa
          // Devolvemos 201 Created con la ubicacion del nuevo recurso
          return Created($"api/Inmuebles/{creado.IdInmueble}", new
          {
            mensaje = "Inmueble creado correctamente (pendiente de habilitacion)",
            inmueble = creado
          });
        }
        catch (Exception ex)
        {
          // Si algo falla (guardar imagen, BD, etc.), deshacemos todo
          transaction.Rollback();
          // Error (ex)
          return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
      }
    }







    //PUT: api/Inmuebles/cambiarEstado
    [HttpPut("cambiarEstado")]
    public IActionResult CambiarEstado([FromBody] InmuebleEstadoDTO dto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var idPropietarioToken = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var ok = _repo.CambiarEstado(dto.IdInmueble, idPropietarioToken, dto.Estado);

      if (!ok)
        return Forbid("No tiene permiso para modificar este inmueble o no existe.");

      return Ok(new
      {
        mensaje = dto.Estado
              ? "Inmueble habilitado correctamente."
              : "Inmueble deshabilitado correctamente.",
        idInmueble = dto.IdInmueble,
        nuevoEstado = dto.Estado
      });

    }

    //GET: api/Inmueble/{id}
    [HttpGet ("{id}")]
    public IActionResult GetInmueble(int id)
    {
      var idPropietario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

      var inmueble = _repo.ObtenerPorId(id, idPropietario);

      if (inmueble == null)
      {
        return NotFound();
      }

      return Ok(inmueble);

    }





  }

}
