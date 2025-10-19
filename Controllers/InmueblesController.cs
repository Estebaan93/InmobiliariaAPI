//Controllers/InmueblesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using InmobiliariaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaAPI.Models;
using InmobiliariaAPI.Models.ViewModels;

namespace InmobiliariaAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class InmueblesController : ControllerBase
  {
    private readonly IInmuebleRepositorio _repo;
    private readonly IWebHostEnvironment _env;
    public InmueblesController(IInmuebleRepositorio repo, IWebHostEnvironment env)
    {
      _repo = repo;
      _env = env;
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

    //GET: api/Inmuebles/activos
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


    //POST: api/Inmuebles/nuevo
    [HttpPost("nuevo")]
    [RequestSizeLimit(10_000_000)] //Hasta 10 MB
    public IActionResult CrearNuevoInmueble([FromBody] InmuebleCrearDTO dto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var idPropietario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


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
        IdDireccion = dto.IdDireccion,
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



  }

}