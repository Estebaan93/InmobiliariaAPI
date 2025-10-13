using InmobiliariaAPI.Data;
using InmobiliariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAPI.Repositories
{
  public class ContratoRepositorio : IContratoRepositorio
  {
    private readonly InmobiliariaContext _context;
    public ContratoRepositorio(InmobiliariaContext context)
    {
      _context = context;
    }

    public IEnumerable<Contrato> ObtenerActivosPorPropietario(int idPropietario)
    {
      return _context.Contratos
        .Include(c => c.Inquilino)
        .Include(c => c.Inmueble)
        .ThenInclude(i => i.Direccion)
        .Where(c => c.Inmueble.IdPropietario == idPropietario && c.Estado)
        .AsNoTracking()
        .ToList();
    }

    public Contrato? ObtenerPorId(int id)
    {
      return _context.Contratos
        .Include(c => c.Inquilino)
        .Include(c => c.Inmueble)
        .FirstOrDefault(c => c.IdContrato == id);
    }
  }
}
