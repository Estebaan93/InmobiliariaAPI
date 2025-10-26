//Models/ViewModels/PropietarioEditarDTO.cs
using System.ComponentModel.DataAnnotations;
namespace InmobiliariaAPI.Models.ViewModels
{
  public class PropietarioEditarDTO
  {
    [RegularExpression(@"^\d{7,10}$", ErrorMessage = "El DNI debe contener solo numeros (7 a 10 dígitos).")]
    public string? Dni { get; set; }

    [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El apellido solo puede contener letras.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
    public string? Apellido { get; set; }

    [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El nombre solo puede contener letras.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string? Nombre { get; set; }

    [RegularExpression(@"^\d{6,12}$", ErrorMessage = "El telefono debe contener solo números (6 a 12 dígitos).")]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "El formato del correo no es valido.")]
    [StringLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres.")]
    public string? Correo { get; set; }
  }
}