using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAPI.Models
{
  [Table("direccion")]
  public class Direccion
  {
    [Key]
    public int IdDireccion { get; set; }

    public string Calle { get; set; } = string.Empty;
    public int Altura { get; set; }
    public string Cp { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string Coordenadas { get; set; } = string.Empty;
  }
}
