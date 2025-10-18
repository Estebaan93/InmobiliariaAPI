//Repositories/IInmuebleRepositorio.cs
using InmobiliariaAPI.Models;

namespace InmobiliariaAPI.Repositories
{
  public interface IInmuebleRepositorio
  {
    List<Inmueble> ObtenerTodosPorPropietario(int idPropietario);
    List<Inmueble> ObtenerActivosPorPropietario(int idPropietario);
    Inmueble? ObtenerPorId(int id);
  }
}
