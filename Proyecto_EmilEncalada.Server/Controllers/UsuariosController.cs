using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_EmilEncalada.Server.Data;
using Proyecto_EmilEncalada.Server.Helpers;
using Proyecto_EmilEncalada.Server.Models;

namespace Proyecto_EmilEncalada.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            return await _context.Usuarios
                .OrderBy(u => u.Nombre)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            return usuario;
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                return BadRequest(new { mensaje = "El nombre es obligatorio." });

            if (string.IsNullOrWhiteSpace(usuario.Correo))
                return BadRequest(new { mensaje = "El correo es obligatorio." });

            if (string.IsNullOrWhiteSpace(usuario.Clave))
                return BadRequest(new { mensaje = "La contraseña es obligatoria." });

            if (usuario.Rol != "Admin" && usuario.Rol != "Usuario")
                return BadRequest(new { mensaje = "El rol debe ser Admin o Usuario." });

            var existe = await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo);

            if (existe)
                return BadRequest(new { mensaje = "El correo ya está registrado." });

            usuario.Activo = true;
            usuario.FechaCreacion = DateTime.Today;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            if (id != usuario.Id)
                return BadRequest(new { mensaje = "El id no coincide." });

            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                return BadRequest(new { mensaje = "El nombre es obligatorio." });

            if (string.IsNullOrWhiteSpace(usuario.Correo))
                return BadRequest(new { mensaje = "El correo es obligatorio." });

            if (string.IsNullOrWhiteSpace(usuario.Clave))
                return BadRequest(new { mensaje = "La contraseña es obligatoria." });

            if (usuario.Rol != "Admin" && usuario.Rol != "Usuario")
                return BadRequest(new { mensaje = "El rol debe ser Admin o Usuario." });

            var existeUsuario = await _context.Usuarios.AnyAsync(u => u.Id == id);

            if (!existeUsuario)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("cambiar-rol/{id}")]
        public async Task<IActionResult> CambiarRol(int id, [FromBody] string nuevoRol)
        {
            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            if (nuevoRol != "Admin" && nuevoRol != "Usuario")
                return BadRequest(new { mensaje = "El rol debe ser Admin o Usuario." });

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            usuario.Rol = nuevoRol;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Rol actualizado correctamente." });
        }

        [HttpPut("estado/{id}")]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            usuario.Activo = !usuario.Activo;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = usuario.Activo ? "Usuario habilitado correctamente." : "Usuario deshabilitado correctamente.",
                activo = usuario.Activo
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}