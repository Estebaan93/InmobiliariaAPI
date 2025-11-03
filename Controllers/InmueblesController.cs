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
    ej si la inmobiliaria solo deja agregar 3 inmuebles por propietario... Como validamos/protegemos la ruta?*/
    [HttpPost("nuevo")]
    [RequestSizeLimit(10_000_000)] //Hasta 10 MB
    [Consumes("multipart/form-data")]
    public IActionResult CrearNuevoInmueble([FromForm] InmuebleCargaDTO carga)
    {
      // Validar que se envió la imagen
      if (carga.Imagen == null || carga.Imagen.Length == 0)
        return BadRequest("La imagen es requerida");

      // Validar que se envió el JSON del inmueble
      if (string.IsNullOrWhiteSpace(carga.Inmueble))
        return BadRequest("Los datos del inmueble son requeridos");
        
      // Deserializar el JSON a InmuebleJsonDTO
      InmuebleJsonDTO dto;
      try
      {
        dto = JsonSerializer.Deserialize<InmuebleJsonDTO>(carga.Inmueble, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });

        if (dto == null)
          return BadRequest("El formato del JSON es invalido");
      }
      catch (JsonException ex)
      {
        return BadRequest($"Error al procesar el JSON: {ex.Message}");
      }

      // Validar el modelo manualmente (ya que viene como string)
      if (string.IsNullOrWhiteSpace(dto.Calle))
        return BadRequest("La calle es requerida");
      
      if (dto.Altura <= 0)
        return BadRequest("La altura debe ser mayor a 0");
      
      if (string.IsNullOrWhiteSpace(dto.Cp))
        return BadRequest("El codigo postal es requerido");
      
      if (string.IsNullOrWhiteSpace(dto.Ciudad))
        return BadRequest("La ciudad es requerida");
      
      if (string.IsNullOrWhiteSpace(dto.Coordenadas))
        return BadRequest("Las coordenadas son requeridas");
      
      if (dto.IdTipo <= 0)
        return BadRequest("El tipo de inmueble es requerido");
      
      if (string.IsNullOrWhiteSpace(dto.Metros2))
        return BadRequest("Los metros cuadrados son requeridos");
      
      if (dto.CantidadAmbientes <= 0)
        return BadRequest("La cantidad de ambientes debe ser mayor a 0");
      
      if (dto.Precio <= 0)
        return BadRequest("El precio debe ser mayor a 0");
      
      if (string.IsNullOrWhiteSpace(dto.Descripcion))
        return BadRequest("La descripcion es requerida");



      var idPropietario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

      // obtener direccion si existe
      string calleTrim = dto.Calle.Trim();
      string cpTrim = dto.Cp.Trim();
      string ciudadTrim = dto.Ciudad.Trim();

      //buscamos
      var direccionExistente = _context.Direcciones.FirstOrDefault(d =>
        d.Calle == calleTrim &&
        d.Altura == dto.Altura &&
        d.Cp == cpTrim &&
        d.Ciudad == ciudadTrim
      );

      Direccion direccionParaInmueble;
      if (direccionExistente != null)
      {
        //Si existe reutilizafmos, puede ser un edificio, torre, shopping etc
        direccionParaInmueble = direccionExistente;
      }
      else
      {
        //crear y guardar la direccion
        var nuevaDireccion = new Direccion
        {
          Calle = calleTrim,
          Altura = dto.Altura,
          Cp = cpTrim,
          Ciudad = ciudadTrim,
          Coordenadas = dto.Coordenadas.Trim()
        };
        _context.Direcciones.Add(nuevaDireccion);
        _context.SaveChanges();

        direccionParaInmueble = nuevaDireccion;

      }

      //Validamos el inmue duplicado
      var inmuebleDuplicado = _context.Inmuebles.FirstOrDefault(i =>
            i.IdPropietario == idPropietario &&
            i.IdDireccion == direccionParaInmueble.IdDireccion &&
            i.IdTipo == dto.IdTipo &&
            i.Metros2 == dto.Metros2 && 
            i.CantidadAmbientes == dto.CantidadAmbientes
        );
      if (inmuebleDuplicado != null)
      {
        // Si encontramos un duplicado, devolvemos un error 409 (conflicto)
        return Conflict(new
        {
          mensaje = "Ya existe un inmueble con caracteristicas identicas (misma direccion, tipo, metros y ambientes) para este propietario.",
          idInmuebleExistente = inmuebleDuplicado.IdInmueble
        });
      }

      //Creamos el inmueble  
      // Guardar imagen en wwwroot/imagenes_inmuebles
      string carpeta = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "imagenes_inmuebles");
      if (!Directory.Exists(carpeta))
        Directory.CreateDirectory(carpeta);

      string nombreArchivo = $"{Guid.NewGuid()}_{carga.Imagen.FileName}";
      string rutaArchivo = Path.Combine(carpeta, nombreArchivo);

      using (var stream = new FileStream(rutaArchivo, FileMode.Create))
      {
        carga.Imagen.CopyTo(stream);
      }

      //Ruta publica (para android)
      string urlImagen = $"{Request.Scheme}://{Request.Host}/imagenes_inmuebles/{nombreArchivo}";

      // Crear el inmueble
      var inmuebleNuevo = new Inmueble
      {
        IdPropietario = idPropietario,
        IdDireccion = direccionParaInmueble.IdDireccion,
        IdTipo = dto.IdTipo,
        Metros2 = dto.Metros2,
        CantidadAmbientes = dto.CantidadAmbientes,
        Precio = dto.Precio,
        Descripcion = dto.Descripcion,
        Cochera = dto.Cochera,
        Piscina = dto.Piscina,
        Mascotas = dto.Mascotas,
        UrlImagen = urlImagen,
        Estado = false //eshabilitado por defecto
      };

      var creado = _repo.CrearInmueble(inmuebleNuevo);

      if (creado == null)
        return StatusCode(500, "Error al crear el inmueble");

      creado.Tipo = _context.Tipos.Find(creado.IdTipo);
      creado.Direccion = direccionParaInmueble;
      return Ok(new
      {
        mensaje = "Inmueble creado correctamente (pendiente de habilitacion)",
        inmueble = creado
      });
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





  }

}
