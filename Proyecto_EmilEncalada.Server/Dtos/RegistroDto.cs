using System.ComponentModel.DataAnnotations;

namespace Proyecto_EmilEncalada.Server.Dtos
{
    public class RegistroDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string Clave { get; set; } = string.Empty;
    }
}
