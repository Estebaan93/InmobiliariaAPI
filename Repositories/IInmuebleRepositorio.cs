//Repositories/IInmuebleRepositorio.cs
using InmobiliariaAPI.Models;
using InmobiliariaAPI.Models.ViewModels;

namespace InmobiliariaAPI.Repositories
{
  public interface IInmuebleRepositorio
  {
    List<Inmueble> ObtenerTodosPorPropietario(int idPropietario);
    List<InmuebleActivoDTO> ObtenerActivosPorPropietario(int idPropietario);
    Inmueble? ObtenerPorId(int id);
    Inmueble? CrearInmueble(Inmueble inmueble);
    bool CambiarEstado(int idInmueble, int idPropietario, bool nuevoEstado);



  }
}
