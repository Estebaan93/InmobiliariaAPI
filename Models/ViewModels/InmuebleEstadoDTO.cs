//Models/ViewModels/InmuebleEstadoDTO.cs
using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAPI.Models.ViewModels
{
  public class InmuebleEstadoDTO
  {
    [Required]
    public int IdInmueble { get; set; }

    [Required]
    public bool Estado { get; set; }
  }
}