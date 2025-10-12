//Models/Propietario.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InmobiliariaAPI.Models
{
  [Table("propietario")]  //fuerza el nombre exacto en la BD
  public class Propietario
  {
    [Key]
    public int IdPropietario { get; set; }

    [Required]
    public string Dni { get; set; } = string.Empty;

    [Required]
    public string Apellido { get; set; } = string.Empty;

    [Required]
    public string Nombre { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] //lo oculta solo al devolver respuesta
    public string Password { get; set; } = string.Empty; // nueva columna

    public bool Estado { get; set; }
  }
}