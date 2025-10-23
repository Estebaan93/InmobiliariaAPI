//Controllers/PropietariosController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using InmobiliariaAPI.Repositories;
using InmobiliariaAPI.Models;
using InmobiliariaAPI.Services;


namespace InmobiliariaAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class PropietariosController : ControllerBase
  {
    private readonly IPropietarioRepositorio _repo;
    private readonly JwtService _jwt;

    public PropietariosController(IPropietarioRepositorio repo, JwtService jwt)
    {
      _repo = repo;
      _jwt = jwt;
    }

    // LOGIN
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
      var propietario = _repo.Login(request.Correo, request.Password);
      if (propietario == null)
        return Unauthorized("Correo o contraseña incorrectos.");

      var token = _jwt.GenerarToken(propietario);
      return Ok(new { token });
    }

    [HttpGet("perfil")]
    [Authorize]
    public IActionResult Perfil()
    {
      var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      var propietario = _repo.ObtenerPorId(id);
      return propietario == null ? NotFound() : Ok(propietario);
    }



    [AllowAnonymous]
    [HttpPost("registro")]
    public IActionResult Registrar([FromBody] Propietario propietario)
    {
      //Valiar correo
      if (string.IsNullOrWhiteSpace(propietario.Correo))
        return BadRequest("Debe ingresa un correo valido");

      if (string.IsNullOrWhiteSpace(propietario.Password))
        return BadRequest("Debe ingresar una contraseña.");

      //Validar si existe el correo
      var existente = _repo.BuscarPorCorreo(propietario.Correo);
      if (existente != null)
        return Conflict("Ya existe el correo electronico");

      //Crea el prop nuevo
      var nuevo = _repo.Registrar(propietario);

      var token = _jwt.GenerarToken(nuevo);

      return Ok(new
      {
        mensaje = "Propietario registrado correctamente",
        propietario = new
        {
          nuevo.IdPropietario,
          nuevo.Nombre,
          nuevo.Apellido,
          nuevo.Correo,
          nuevo.Estado,
          nuevo.UltimoCambioPassword
        },
        token
      });
    }



    [HttpPut("editar")]
    [Authorize]
    public IActionResult Editar([FromBody] Propietario propietario)
    {
      var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
      propietario.IdPropietario = id;

      var ok = _repo.Actualizar(propietario);
      return ok ? Ok("Datos actualizados") : NotFound("No se encontró el propietario");
    }

    [HttpPut("cambiarpassword")]
    [Authorize]
    public IActionResult CambiarPassword([FromBody] CambioPasswordRequest req)
    {
    var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    var propietario = _repo.ObtenerPorId(id);
    if (propietario == null)
        return NotFound("Propietario no encontrado");

    var ok = _repo.CambiarPassword(id, req.ClaveActual, req.ClaveNueva);
    if (!ok)
        return BadRequest("Contraseña actual incorrecta");

    //Generamos un nuevo token para reemplazar el anterior
    var nuevoToken = _jwt.GenerarToken(propietario);

    return Ok(new 
    { 
        mensaje = "Contraseña actualizada correctamente", 
        token = nuevoToken 
    });
    }
  }

  public class LoginRequest
  {
    public string Correo { get; set; }
    public string Password { get; set; }
  }

  public class CambioPasswordRequest
  {
    public string ClaveActual { get; set; }
    public string ClaveNueva { get; set; }
  }
}