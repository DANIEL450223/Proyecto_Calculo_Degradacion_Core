using Microsoft.AspNetCore.Mvc;

namespace Proyecto_EmilEncalada.Server.Helpers
{
    public static class SeguridadHelper
    {
        public static bool EsAdmin(HttpRequest request)
        {
            if (!request.Headers.ContainsKey("X-Rol"))
                return false;

            string rol = request.Headers["X-Rol"].ToString().Trim();

            return rol.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        public static bool EstaAutenticado(HttpRequest request)
        {
            if (!request.Headers.ContainsKey("X-Rol"))
                return false;

            string rol = request.Headers["X-Rol"].ToString().Trim();

            return rol.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                || rol.Equals("Usuario", StringComparison.OrdinalIgnoreCase);
        }

        public static ActionResult AccesoDenegado()
        {
            return new UnauthorizedObjectResult(new
            {
                mensaje = "No tiene permisos para realizar esta acción. Solo el administrador puede hacerlo."
            });
        }

        public static ActionResult DebeIniciarSesion()
        {
            return new UnauthorizedObjectResult(new
            {
                mensaje = "Debe iniciar sesión para acceder a esta información."
            });
        }
    }
}