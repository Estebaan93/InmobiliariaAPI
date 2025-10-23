//Repositories/IPropietarioRepositorio.cs
using InmobiliariaAPI.Models;

namespace InmobiliariaAPI.Repositories
{
  public interface IPropietarioRepositorio
  {
    Propietario Login(string correo, string password);
    Propietario ObtenerPorId(int idPropietario);
    Propietario Registrar(Propietario propietario);
    bool Actualizar(Propietario propietario);
    Propietario BuscarPorCorreo(string correo);
    bool CambiarPassword(int id, string claveActual, string claveNueva);
  }

}