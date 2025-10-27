//Repositories/ContratoRepositorio.cs
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

    public IEnumerable<Contrato> ObtenerPorInmueble(int idInmueble, int idPropietario)
    {
      //Validamos que el inmu pertenece al propietario loguado
      var inmueble = _context.Inmuebles
                          .AsNoTracking()
                          .FirstOrDefault(i => i.IdInmueble == idInmueble && i.IdPropietario == idPropietario);

      //Si no existe o no pertenece devuelve lista vvacia
      if (inmueble == null)
      {
        return new List<Contrato>();
      }
      
      //Si pertenece, busca sus contratos con inquilino y pagos
      
      return _context.Contratos
          .Include(c=>c.Inquilino) //inquilino
          .Include(c=> c.Pagos) //lista de pagos
          .Where(c=>c.IdInmueble==idInmueble)
          .OrderByDescending(c => c.FechaInicio)
          .AsNoTracking()
          .ToList();     

    }





  }
}
