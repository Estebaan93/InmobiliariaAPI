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
    [RegularExpression(@"^\d{7,10}$", ErrorMessage = "El DNI debe contener solo numeros (7 a 10 dígitos).")]
    public string Dni { get; set; } = string.Empty;

    [Required]
    //Solo letras
    public string Apellido { get; set; } = string.Empty;

    [Required]
    //Solo letras
    public string Nombre { get; set; } = string.Empty;



    //Solo numeros
    [RegularExpression(@"^\d{6,12}$", ErrorMessage = "El telefono debe contener solo numeros (7 a 10 dígitos).")]
    //si no se llena el campo del telefono, viaja vacio y en la BD el campo que vacio
    public string Telefono { get; set; } = string.Empty;

    //Validar email
    [Required, EmailAddress]
    public string Correo { get; set; } = string.Empty;

    //[Required] para el endpoint de editar perfil
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] //lo oculta solo al devolver respuesta
    public string Password { get; set; } = string.Empty; // nueva columna

    public bool Estado { get; set; }
    public DateTime UltimoCambioPassword { get; set; } = DateTime.UtcNow;
  }
}