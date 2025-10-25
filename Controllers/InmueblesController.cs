//Controllers/InmueblesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using InmobiliariaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaAPI.Models;
using InmobiliariaAPI.Models.ViewModels;
using InmobiliariaAPI.Data;

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


    //POST: api/Inmuebles/nuevo cargar inmueble 
    /*Validar el propietario logueado y que el inmueble a agregar le pertenezca, ej puede otro propietario con su token agregar inmuebles a su nombre
    ej si la inmobiliaria solo deja agregar 3 inmuebles por propietario... Como validamos/protegemos la ruta?*/
    [HttpPost("nuevo")]
    [RequestSizeLimit(10_000_000)] //Hasta 10 MB
    public IActionResult CrearNuevoInmueble([FromForm] InmuebleCrearDTO dto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var idPropietario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

      //crear y guardar la direccion
      var nuevaDireccion = new Direccion
      {
        Calle = dto.Calle,
        Altura = dto.Altura,
        Cp = dto.Cp,
        Ciudad = dto.Ciudad,
        Coordenadas = dto.Coordenadas
      };

      // Guardar imagen en wwwroot/imagenes_inmuebles
      string carpeta = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "imagenes_inmuebles");
      if (!Directory.Exists(carpeta))
        Directory.CreateDirectory(carpeta);

      string nombreArchivo = $"{Guid.NewGuid()}_{dto.Imagen.FileName}";
      string rutaArchivo = Path.Combine(carpeta, nombreArchivo);

      using (var stream = new FileStream(rutaArchivo, FileMode.Create))
      {
        dto.Imagen.CopyTo(stream);
      }

      //Ruta publica (para android)
      string urlImagen = $"{Request.Scheme}://{Request.Host}/imagenes_inmuebles/{nombreArchivo}";

      // Crear el inmueble
      var inmueble = new Inmueble
      {
        IdPropietario = idPropietario,
        IdDireccion = nuevaDireccion.IdDireccion,
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

      var creado = _repo.CrearInmueble(inmueble);

      if (creado == null)
        return StatusCode(500, "Error al crear el inmueble");

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
