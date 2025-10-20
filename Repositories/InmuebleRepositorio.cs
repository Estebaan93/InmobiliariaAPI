//Repositories/InmuebleRepositorio.cs
using InmobiliariaAPI.Data;
using InmobiliariaAPI.Models;
using InmobiliariaAPI.Models.ViewModels;
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


    // DTO: inmuebles con contratos vigentes e inquilino
    public List<InmuebleActivoDTO> ObtenerActivosPorPropietario(int idPropietario)
    {
      var hoy = DateTime.Today;

      return _context.Contratos
        .Include(c => c.Inmueble!)
          .ThenInclude(i => i.Direccion)
        .Include(c => c.Inmueble!)
          .ThenInclude(i => i.Tipo)
        .Include(c => c.Inquilino)
        .Where(c => c.Estado
                    && c.FechaInicio <= hoy
                    && c.FechaFin >= hoy
                    && c.Inmueble!.IdPropietario == idPropietario)
        .Select(c => new InmuebleActivoDTO
        {
          IdInmueble = c.Inmueble!.IdInmueble,
          Descripcion = c.Inmueble!.Descripcion,
          Calle = c.Inmueble!.Direccion!.Calle,
          Ciudad = c.Inmueble!.Direccion!.Ciudad,
          Tipo = c.Inmueble!.Tipo!.Observacion,
          UrlImagen = c.Inmueble!.UrlImagen,
          InquilinoNombre = c.Inquilino!.Nombre,
          InquilinoApellido = c.Inquilino!.Apellido,
          FechaInicio = c.FechaInicio,
          FechaFin = c.FechaFin,
          Monto = c.Monto
        })
        .AsNoTracking()
        .ToList();
    }


    //Crear inmueble
    public Inmueble? CrearInmueble(Inmueble inmueble)
    {
      try
      {
        _context.Inmuebles.Add(inmueble);
        _context.SaveChanges();
        return inmueble;
      }
      catch (Exception)
      {
        return null;
      }



    }

  }
}
