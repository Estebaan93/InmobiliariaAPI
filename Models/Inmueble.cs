using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InmobiliariaAPI.Models
{
  [Table("inmueble")]
  public class Inmueble
  {
    [Key]
    public int IdInmueble { get; set; }

    [ForeignKey("Propietario")]
    public int IdPropietario { get; set; }

    [ForeignKey("Direccion")]
    public int IdDireccion { get; set; }

    [ForeignKey("Tipo")]
    public int IdTipo { get; set; }

    public string Metros2 { get; set; } = string.Empty;
    public int CantidadAmbientes { get; set; }
    public decimal Precio { get; set; }
    public string Descripcion { get; set; } = string.Empty;

    public bool Cochera { get; set; }
    public bool Piscina { get; set; }
    public bool Mascotas { get; set; }

    public string UrlImagen { get; set; } = string.Empty;
    public bool Estado { get; set; }

    [JsonIgnore] public Propietario? Propietario { get; set; }
    public Direccion? Direccion { get; set; }
    public Tipo? Tipo { get; set; }

    [JsonIgnore] public ICollection<Contrato>? Contratos { get; set; }
  }
}
