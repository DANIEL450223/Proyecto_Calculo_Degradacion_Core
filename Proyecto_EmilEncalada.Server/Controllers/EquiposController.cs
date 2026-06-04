using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_EmilEncalada.Server.Data;
using Proyecto_EmilEncalada.Server.Models;
using Proyecto_EmilEncalada.Server.Helpers;

namespace Proyecto_EmilEncalada.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquiposController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EquiposController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Equipo>>> GetEquipos()
        {
            return await _context.Equipos
                .Include(e => e.Departamento)
                .OrderBy(e => e.Nombre)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Equipo>> GetEquipo(int id)
        {
            var equipo = await _context.Equipos
                .Include(e => e.Departamento)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipo == null)
                return NotFound(new { mensaje = "Equipo no encontrado." });

            return equipo;
        }

        [HttpPost]
        public async Task<ActionResult<Equipo>> PostEquipo(Equipo equipo)
        {
            if (!SeguridadHelper.EstaAutenticado(Request))
                return SeguridadHelper.DebeIniciarSesion();

            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            if (equipo.FechaCompra.Date > DateTime.Today)
                return BadRequest(new { mensaje = "La fecha de compra no puede ser futura." });

            if (equipo.CostoInicial <= 0)
                return BadRequest(new { mensaje = "El costo inicial debe ser mayor a cero." });

            if (equipo.VidaUtilMeses <= 0)
                return BadRequest(new { mensaje = "La vida útil debe ser mayor a cero." });

            if (string.IsNullOrWhiteSpace(equipo.Nombre))
                return BadRequest(new { mensaje = "El nombre del equipo es obligatorio." });

            if (string.IsNullOrWhiteSpace(equipo.Tipo))
                return BadRequest(new { mensaje = "El tipo del equipo es obligatorio." });

            if (string.IsNullOrWhiteSpace(equipo.Marca))
                return BadRequest(new { mensaje = "La marca del equipo es obligatoria." });

            if (string.IsNullOrWhiteSpace(equipo.Estado))
                return BadRequest(new { mensaje = "El estado del equipo es obligatorio." });

            var departamentoExiste = await _context.Departamentos
                .AnyAsync(d => d.Id == equipo.DepartamentoId);

            if (!departamentoExiste)
                return BadRequest(new { mensaje = "El departamento seleccionado no existe." });

            _context.Equipos.Add(equipo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEquipo), new { id = equipo.Id }, equipo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipo(int id, Equipo equipo)
        {
            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            if (id != equipo.Id)
                return BadRequest(new { mensaje = "El id no coincide." });

            if (equipo.FechaCompra.Date > DateTime.Today)
                return BadRequest(new { mensaje = "La fecha de compra no puede ser futura." });

            if (equipo.CostoInicial <= 0)
                return BadRequest(new { mensaje = "El costo inicial debe ser mayor a cero." });

            if (equipo.VidaUtilMeses <= 0)
                return BadRequest(new { mensaje = "La vida útil debe ser mayor a cero." });

            if (string.IsNullOrWhiteSpace(equipo.Nombre))
                return BadRequest(new { mensaje = "El nombre del equipo es obligatorio." });

            if (string.IsNullOrWhiteSpace(equipo.Tipo))
                return BadRequest(new { mensaje = "El tipo del equipo es obligatorio." });

            if (string.IsNullOrWhiteSpace(equipo.Marca))
                return BadRequest(new { mensaje = "La marca del equipo es obligatoria." });

            if (string.IsNullOrWhiteSpace(equipo.Estado))
                return BadRequest(new { mensaje = "El estado del equipo es obligatorio." });

            var departamentoExiste = await _context.Departamentos
                .AnyAsync(d => d.Id == equipo.DepartamentoId);

            if (!departamentoExiste)
                return BadRequest(new { mensaje = "El departamento seleccionado no existe." });

            var existeEquipo = await _context.Equipos.AnyAsync(e => e.Id == id);

            if (!existeEquipo)
                return NotFound(new { mensaje = "Equipo no encontrado." });

            _context.Entry(equipo).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipo(int id)
        {
            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            var equipo = await _context.Equipos.FindAsync(id);

            if (equipo == null)
                return NotFound(new { mensaje = "Equipo no encontrado." });

            _context.Equipos.Remove(equipo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}