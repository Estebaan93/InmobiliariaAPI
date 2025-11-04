//Repositories/PagoRepositorio.cs
using InmobiliariaAPI.Data;
using InmobiliariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAPI.Repositories
{
  public class PagoRepositorio : IPagoRepositorio
  {
    private readonly InmobiliariaContext _context;
    public PagoRepositorio(InmobiliariaContext context)
    {
      _context = context;
    }

    public IEnumerable<Pago> ObtenerPorContrato(int idContrato, int idPropietario)
    {
      // Validar que el contrato existe y pertenece al propietario
      var contrato = _context.Contratos
          .Include(c => c.Inmueble)  // Necesitamos el inmueble para validar el propietario
          .AsNoTracking()
          .FirstOrDefault(c => c.IdContrato == idContrato 
                             && c.Inmueble.IdPropietario == idPropietario);

      // Si no existe o no pertenece al propietario, retornar lista vacia
      if (contrato == null)
      {
        return new List<Pago>();
      }

      // Solo si el contrato pertenece al propietario, retornar sus pagos
      return _context.Pagos
          .Where(p => p.IdContrato == idContrato)
          .OrderBy(p => p.FechaPago)
          .AsNoTracking()
          .ToList();
    }
  }
}
