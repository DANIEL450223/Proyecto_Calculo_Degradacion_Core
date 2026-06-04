using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_EmilEncalada.Server.Data;
using Proyecto_EmilEncalada.Server.Dtos;
using Proyecto_EmilEncalada.Server.Models;
using Proyecto_EmilEncalada.Server.Helpers;

namespace Proyecto_EmilEncalada.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(RegistroDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "El nombre es obligatorio." });

            if (string.IsNullOrWhiteSpace(dto.Correo))
                return BadRequest(new { mensaje = "El correo es obligatorio." });

            if (string.IsNullOrWhiteSpace(dto.Clave))
                return BadRequest(new { mensaje = "La contraseña es obligatoria." });

            var existe = await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo);

            if (existe)
                return BadRequest(new { mensaje = "El correo ya está registrado." });

            string rolAsignado = "Usuario";

            if (!string.IsNullOrWhiteSpace(dto.Rol))
            {
                if (dto.Rol != "Admin" && dto.Rol != "Usuario")
                    return BadRequest(new { mensaje = "El rol ingresado no es válido." });

                if (dto.Rol == "Admin")
                {
                    if (!SeguridadHelper.EsAdmin(Request))
                        return SeguridadHelper.AccesoDenegado();

                    rolAsignado = "Admin";
                }
                else
                {
                    rolAsignado = "Usuario";
                }
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                Clave = dto.Clave,
                Rol = rolAsignado
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Usuario registrado correctamente",
                usuario = new
                {
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Correo,
                    usuario.Rol
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == dto.Correo && u.Clave == dto.Clave);

            if (usuario == null)
                return Unauthorized(new { mensaje = "Correo o contraseña incorrectos." });

            if (!usuario.Activo)
                return Unauthorized(new { mensaje = "El usuario está deshabilitado. Contacte al administrador." });

            return Ok(new
            {
                mensaje = "Login correcto",
                usuario = new
                {
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Correo,
                    usuario.Rol,
                    usuario.Activo,
                    usuario.FechaCreacion
                }
            });
        }
    }
}