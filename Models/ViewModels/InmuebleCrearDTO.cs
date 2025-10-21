//Models/ViewModels/InmuebleCrearDTO.cs
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAPI.Models.ViewModels
{
  public class InmuebleCrearDTO
  {
    //Campos de direccion
    [Required]
    public string Calle { get; set; } = string.Empty;

    [Required]
    public int Altura{ get; set; }

    [Required]
    public string Cp { get; set; }= string.Empty;

    [Required]
    public string Ciudad { get; set; }= string.Empty;

    [Required]
    public string Coordenadas { get; set; }= string.Empty;


    //Campos de inmuebles
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
