//Repositories/IInmuebleRepositorio.cs
using InmobiliariaAPI.Models;

namespace InmobiliariaAPI.Repositories
{
  public interface IInmuebleRepositorio
  {
    List<Inmueble> ObtenerPorPropietario(int idPropietario);
    Inmueble? ObtenerPorId(int id);
  }
}
