using System.ComponentModel.DataAnnotations;

namespace Proyecto_EmilEncalada.Server.Dtos
{
    public class LoginDto
    {
        [Required]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string Clave { get; set; } = string.Empty;
    }
}
