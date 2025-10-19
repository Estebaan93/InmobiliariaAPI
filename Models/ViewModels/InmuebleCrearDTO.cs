//Models/ViewModels/InmuebleCrearDTO.cs
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAPI.Models.ViewModels
{
  public class InmuebleCrearDTO
  {
    [Required]
    public int IdDireccion { get; set; }

    [Required]
    public int IdTipo { get; set; }

    [Required]
    public string Metros2 { get; set; } = string.Empty;

    [Required]
    public int CantidadAmbientes { get; set; }

    [Required]
    public decimal Precio { get; set; }

    [Required]
    public string Descripcion { get; set; } = string.Empty;

    public bool Cochera { get; set; }
    public bool Piscina { get; set; }
    public bool Mascotas { get; set; }

    // Imagen subida desde Android
    [Required]
    public IFormFile Imagen { get; set; } = null!;
  }
}
