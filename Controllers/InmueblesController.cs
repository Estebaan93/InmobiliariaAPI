//Controllers/InmueblesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using InmobiliariaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class InmueblesController: ControllerBase
  {
    private readonly IInmuebleRepositorio _repo;
    public InmueblesController(IInmuebleRepositorio repo)
    {
      _repo = repo;
    }

    //GET: api/Inmuebles/obtenerInmueble (obtiene inmuebles del propietario)
    [HttpGet("obtenerInmuebles")]
    public IActionResult ObtenerPorPropietario()
    {
      //Obtenemos el id del propietario logueado desde el token
      var idPropietario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

      var inmuebles = _repo.ObtenerPorPropietario(idPropietario);
      if(inmuebles ==null || inmuebles.Count == 0)
      {
        return NotFound("No se encontraron inmuebles para este propietario");

      }
      return Ok(inmuebles);
    }
    
  }



}