//Models/ViewModels/InmuebleJsonDTO.cs
using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAPI.Models.ViewModels
{
  public class InmuebleJsonDTO
  {
    //Campos de direccion
    [Required(ErrorMessage = "La calle es requerida")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "La calle debe tener entre 3 y 100 caracteres")]
    public string Calle { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La altura debe ser un numero positivo")]
    public int Altura { get; set; }

    [Required(ErrorMessage = "El codigo postal es requerido")]
    [StringLength(10, MinimumLength = 4, ErrorMessage = "El CP debe tener entre 4 y 10 caracteres")]
    public string Cp { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ciudad es requerida")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "La ciudad debe tener entre 3 y 50 caracteres")]
    public string Ciudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Las coordenadas son requeridas")]
    public string Coordenadas { get; set; } = string.Empty;


    //Campos de inmuebles
    [Range(1, int.MaxValue, ErrorMessage = "El tipo de inmueble es requerido")]
    public int IdTipo { get; set; }

    [Required(ErrorMessage = "Los metros cuadrados son requeridos")]
    public string Metros2 { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "La cantidad de ambientes debe ser entre 1 y 20")]
    public int CantidadAmbientes { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "La descripcion es requerida")]
    [StringLength(500, ErrorMessage = "La descripcion no puede exceder los 500 caracteres")]
    public string Descripcion { get; set; } = string.Empty;

    public bool Cochera { get; set; }
    public bool Piscina { get; set; }
    public bool Mascotas { get; set; }

    // NO incluye la imagen - se recibe por separado
  }
}