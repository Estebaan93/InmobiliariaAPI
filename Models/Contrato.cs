//Model/Contrato.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InmobiliariaAPI.Models
{
  [Table("contrato")]
  public class Contrato
  {
    [Key]
    public int IdContrato { get; set; }

    [ForeignKey("Inquilino")]
    public int IdInquilino { get; set; }

    [ForeignKey("Inmueble")]
    public int IdInmueble { get; set; }

    public decimal Monto { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public DateTime? FechaAnulacion { get; set; }
    public bool Estado { get; set; }

    public Inquilino? Inquilino { get; set; }

    [JsonIgnore]
    public Inmueble? Inmueble { get; set; }

    public ICollection<Pago>? Pagos { get; set; }
  }
}
