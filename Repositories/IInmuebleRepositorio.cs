using InmobiliariaAPI.Models;

namespace InmobiliariaAPI.Repositories
{
  public interface IInmuebleRepositorio
  {
    IEnumerable<Inmueble> ObtenerPorPropietario(int idPropietario);
    Inmueble? ObtenerPorId(int id);
  }
}
