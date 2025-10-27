//Model/Inquilino.cs
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InmobiliariaAPI.Models
{
  [Table("inquilino")]
  public class Inquilino
  {
    [Key]
    public int IdInquilino { get; set; }
    [Required]

    public string Dni { get; set; }

    [Required]
    public string Apellido { get; set; } = string.Empty;

    [Required]
    public string Nombre { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Correo { get; set; } = string.Empty;

    public bool Estado { get; set; } = true;
    
    [JsonIgnore]
    public ICollection<Contrato>? Contratos { get; set; }

  }
}