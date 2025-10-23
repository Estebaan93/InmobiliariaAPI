//Services/JwtService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using InmobiliariaAPI.Models;

namespace InmobiliariaAPI.Services
{
  public class JwtService
  {
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
      _config = config;
    }

    public string GenerarToken(Propietario prop)
    {
      var claims = new[]
      {
                new Claim(ClaimTypes.NameIdentifier, prop.IdPropietario.ToString()),
                new Claim(ClaimTypes.Email, prop.Correo),
                new Claim(ClaimTypes.Name, $"{prop.Nombre} {prop.Apellido}"),
                new Claim("LastPasswordChange", prop.UltimoCambioPassword.ToString("O"))
            };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: _config["Jwt:Issuer"],
          audience: _config["Jwt:Audience"],
          claims: claims,
          expires: DateTime.UtcNow.AddHours(1),
          signingCredentials: creds
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}
