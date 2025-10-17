//Model/Inquilino.cs
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAPI.Models
{
  [Table("inquilino")]
  public class Inquilino
  {
    [Key]
    public int IdInquilino { get; set; }
    [Required]

    public int Dni { get; set; }

    [Required]
    public string Apellido { get; set; } = string.Empty;

    [Required]
    public string Nombre { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Correo { get; set; } = string.Empty;

    public bool Estado { get; set; } = true;
    
    public ICollection<Contrato>? Contratos { get; set; }

  }
}