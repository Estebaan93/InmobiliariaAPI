namespace InmobiliariaAPI.Models.ViewModels
{
  public class InmuebleActivoDTO
  {
    public int IdInmueble { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Calle { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string UrlImagen { get; set; } = string.Empty;
    public string InquilinoNombre { get; set; } = string.Empty;
    public string InquilinoApellido { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal Monto { get; set; }
  }
}
