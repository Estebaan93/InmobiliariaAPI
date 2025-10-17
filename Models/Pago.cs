//Model/Pago.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAPI.Models
{
  [Table("pago")]
  public class Pago
  {
    [Key]
    public int IdPago { get; set; }

    [ForeignKey("Contrato")]
    public int IdContrato { get; set; }

    public DateTime FechaPago { get; set; }
    public decimal Importe { get; set; }
    public string NumeroPago { get; set; } = string.Empty;
    public string Detalle { get; set; } = string.Empty;
    public bool Estado { get; set; }

    public Contrato? Contrato { get; set; }
  }
}
