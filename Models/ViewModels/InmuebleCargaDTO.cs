// En: Models/ViewModels/InmuebleCargaDTO.cs
using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAPI.Models.ViewModels
{
    public class InmuebleCargaDTO
    {
        [Required(ErrorMessage = "La imagen es requerida")]
        public IFormFile Imagen { get; set; }

        [Required(ErrorMessage = "Los datos del inmueble son requeridos")]
        public string Inmueble { get; set; } // Aqui vendra el string JSON
    }
}