using System.ComponentModel.DataAnnotations;

namespace Proyecto_EmilEncalada.Server.Dtos
{
    public class RegistroDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Clave { get; set; } = string.Empty;

        public string Rol { get; set; } = "Usuario";
    }
}