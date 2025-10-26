//Model/Direccion.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAPI.Models
{
  [Table("direccion")]
  public class Direccion
  {
    [Key]
    public int IdDireccion { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Calle { get; set; } = string.Empty;

    [Required]
    public int Altura { get; set; }

    [Required]
    [StringLength(12)]
    public string Cp { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Ciudad { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    public string Coordenadas { get; set; } = string.Empty;
  }
}
