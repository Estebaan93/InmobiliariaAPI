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
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El apellido solo puede contener letras.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El nombre solo puede contener letras.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;



    //Solo numeros
    [RegularExpression(@"^\d{6,12}$", ErrorMessage = "El telefono debe contener solo numeros (7 a 10 dígitos).")]
    //si no se llena el campo del telefono, en la BD se mantiene el mismo
    public string Telefono { get; set; } = string.Empty;

    //Validar correo
    [Required]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
    [StringLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres.")]
    public string Correo { get; set; } = string.Empty;

    //[Required] para el endpoint de editar perfil
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] //lo oculta solo al devolver respuesta
    public string Password { get; set; } = string.Empty; // nueva columna

    public bool Estado { get; set; }
    public DateTime UltimoCambioPassword { get; set; } = DateTime.UtcNow;
  }
}