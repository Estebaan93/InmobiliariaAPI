//Repositories/IContratoRepositorio.cs
using InmobiliariaAPI.Models;

namespace InmobiliariaAPI.Repositories
{
  public interface IContratoRepositorio
  {
    IEnumerable<Contrato> ObtenerActivosPorPropietario(int idPropietario);
    Contrato? ObtenerPorId(int id);
    IEnumerable<Contrato> ObtenerPorInmueble(int idInmueble, int idPropietario);



  }
}
