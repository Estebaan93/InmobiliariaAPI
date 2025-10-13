using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAPI.Models
{
  [Table("tipo")]
  public class Tipo
  {
    [Key]
    public int IdTipo { get; set; }

    public string Observacion { get; set; } = string.Empty;
  }
}
