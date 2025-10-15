//Repositories/PropietarioRepositorio.cs
using InmobiliariaAPI.Data;
using InmobiliariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAPI.Repositories
{
  public class PropietarioRepositorio : IPropietarioRepositorio
  {
    private readonly InmobiliariaContext _context;
    public PropietarioRepositorio(InmobiliariaContext context)
    {
      _context = context;
    }


    public Propietario Login(string correo, string clave)
    {
      var propietario = _context.Propietarios
        .AsNoTracking()
        .FirstOrDefault(p => p.Correo == correo && p.Estado);

      if (propietario == null)
        return null;

      //verificamos la contras
      bool valido = BCrypt.Net.BCrypt.Verify(clave, propietario.Password);
      return valido ? propietario : null;

    }



    public Propietario ObtenerPorId(int id)
    {
      return _context.Propietarios.AsNoTracking().FirstOrDefault(p => p.IdPropietario == id);
    }


    public Propietario Registrar(Propietario propietario)
    {
      propietario.Password = BCrypt.Net.BCrypt.HashPassword(propietario.Password);
      _context.Propietarios.Add(propietario);
      _context.SaveChanges();
      return propietario;
    }


    public bool Actualizar(Propietario propietario)
    {
      var existente = _context.Propietarios.Find(propietario.IdPropietario);
      if (existente == null) return false;
      
      existente.Dni = propietario.Dni;
      existente.Nombre = propietario.Nombre;
      existente.Apellido = propietario.Apellido;
      existente.Telefono = propietario.Telefono;
      existente.Correo = propietario.Correo;
      existente.Estado = propietario.Estado;

      _context.Update(existente);
      return _context.SaveChanges() > 0;
    }

    public bool CambiarPassword(int id, string claveActual, string claveNueva)
    {
      var propietario = _context.Propietarios.Find(id);
      if (propietario == null) return false;

      if (!BCrypt.Net.BCrypt.Verify(claveActual, propietario.Password))
        return false;

      propietario.Password = BCrypt.Net.BCrypt.HashPassword(claveNueva);
      _context.Update(propietario);
      return _context.SaveChanges() > 0;
    }


  }

}