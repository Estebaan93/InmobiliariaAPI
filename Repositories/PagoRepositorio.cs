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

    public IEnumerable<Pago> ObtenerPorContrato(int idContrato)
    {
      return _context.Pagos
        .Where(p => p.IdContrato == idContrato)
        .AsNoTracking()
        .ToList();
    }
  }
}
