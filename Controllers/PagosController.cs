//Controllers/PagosController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using InmobiliariaAPI.Repositories;

namespace InmobiliariaAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class PagosController : ControllerBase
  {
    private readonly IPagoRepositorio _pagoRepo;

    public PagosController(IPagoRepositorio pagoRepo)
    {
      _pagoRepo = pagoRepo;
    }
    // GET: api/Pagos/porContrato/5
    [HttpGet("porContrato/{idContrato}")]
    public IActionResult GetPagosPorContrato(int idContrato)
    {
      // Obtener el ID del propietario desde el token JWT
      // Este ID NO puede ser manipulado por el cliente
      var idPropietario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

      // Llamar al repositorio pasando AMBOS parsmetros
      // El repositorio se encarga de validar que el contrato pertenece al propietario
      var pagos = _pagoRepo.ObtenerPorContrato(idContrato, idPropietario);

      // Retornar los pagos
      // Si no tenía permiso, el repositorio ya retorno una lista vacia
      return Ok(pagos);
    }
  }
  

}