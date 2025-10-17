//Repositories/InmuebleRepositorio.cs
using InmobiliariaAPI.Data;
using InmobiliariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAPI.Repositories
{
  public class InmuebleRepositorio : IInmuebleRepositorio
  {
    private readonly InmobiliariaContext _context;

    public InmuebleRepositorio(InmobiliariaContext context)
    {
      _context = context;
    }

    public IEnumerable<Inmueble> ObtenerPorPropietario(int idPropietario)
    {
      return _context.Inmuebles
        .Include(i => i.Direccion)
        .Include(i => i.Tipo)
        .Where(i => i.IdPropietario == idPropietario)
        .AsNoTracking()
        .ToList();
    }

    public Inmueble? ObtenerPorId(int id)
    {
      return _context.Inmuebles
        .Include(i => i.Direccion)
        .Include(i => i.Tipo)
        .Include(i => i.Contratos)
        .FirstOrDefault(i => i.IdInmueble == id);
    }
  }
}
