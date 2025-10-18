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

    //Todos los inmuebles del propietario
    public List<Inmueble> ObtenerTodosPorPropietario(int idPropietario)
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
        .Include(i => i.Contratos!)
          .ThenInclude(c => c.Inquilino)
        .Include(i => i.Contratos!)
          .ThenInclude(c => c.Pagos)
        .FirstOrDefault(i => i.IdInmueble == id);
    }

    
    //Solo activos con contratos, pago y datos del inquilino
    public List<Inmueble> ObtenerActivosPorPropietario (int idPropietario)
    {
      return _context.Inmuebles
        .Include(i => i.Direccion)
        .Include(i => i.Tipo)
        .Include(i => i.Contratos!)
          .ThenInclude(c => c.Inquilino)
        .Include(i => i.Contratos!)
          .ThenInclude(c => c.Pagos)
        .Where(i => i.IdPropietario == idPropietario && i.Estado)
        .AsNoTracking()
        .ToList();
    }

  }
}
