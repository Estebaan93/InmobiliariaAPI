//Controllers/ContratosController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using InmobiliariaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaAPI.Models;


namespace InmobiliariaAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize] // Requiere autenticación en todos los metodos
  public class ContratosController : ControllerBase
  {
    private readonly IContratoRepositorio _contratoRepo;

    public ContratosController(IContratoRepositorio contratoRepo)
    {
      _contratoRepo = contratoRepo;
    }

    // GET: api/Contratos/porInmueble/5
    [HttpGet("porInmueble/{idInmueble}")]
    public IActionResult GetContratosPorInmueble(int idInmueble)
    {
      //Obtenemos el id del propietario logueado desde el token
      var idPropietario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

      // Llamamos al repositorio
      // La validacion de pertenencia ya se hace DENTRO del repositorio
      var contratos = _contratoRepo.ObtenerPorInmueble(idInmueble, idPropietario);

      // Devolvemos los contratos
      // (Si no encontro nada o no le pertenece, devolvera una lista vacia)
      return Ok(contratos);
    }
  }
}