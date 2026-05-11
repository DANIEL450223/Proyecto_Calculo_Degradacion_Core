using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_EmilEncalada.Server.Data;
using Proyecto_EmilEncalada.Server.Models;

namespace Proyecto_EmilEncalada.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReparacionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReparacionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reparacion>>> GetReparaciones() =>
            await _context.Reparaciones.Include(r => r.Equipo).OrderByDescending(r => r.FechaReparacion).ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Reparacion>> GetReparacion(int id)
        {
            var reparacion = await _context.Reparaciones.Include(r => r.Equipo).FirstOrDefaultAsync(r => r.Id == id);
            return reparacion == null ? NotFound() : reparacion;
        }

        [HttpPost]
        public async Task<ActionResult<Reparacion>> PostReparacion(Reparacion reparacion)
        {
            if (reparacion.FechaReparacion > DateTime.Now)
                return BadRequest(new { mensaje = "La fecha de reparación no puede ser futura." });

            if (reparacion.Costo <= 0)
                return BadRequest(new { mensaje = "El costo debe ser mayor a cero." });

            var equipoExiste = await _context.Equipos.AnyAsync(e => e.Id == reparacion.EquipoId);
            if (!equipoExiste)
                return BadRequest(new { mensaje = "El equipo seleccionado no existe." });

            _context.Reparaciones.Add(reparacion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReparacion), new { id = reparacion.Id }, reparacion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReparacion(int id, Reparacion reparacion)
        {
            if (id != reparacion.Id)
                return BadRequest(new { mensaje = "El id no coincide." });

            if (reparacion.FechaReparacion > DateTime.Now)
                return BadRequest(new { mensaje = "La fecha de reparación no puede ser futura." });

            if (reparacion.Costo <= 0)
                return BadRequest(new { mensaje = "El costo debe ser mayor a cero." });

            var equipoExiste = await _context.Equipos.AnyAsync(e => e.Id == reparacion.EquipoId);
            if (!equipoExiste)
                return BadRequest(new { mensaje = "El equipo seleccionado no existe." });

            _context.Entry(reparacion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReparacion(int id)
        {
            var reparacion = await _context.Reparaciones.FindAsync(id);
            if (reparacion == null) return NotFound();
            _context.Reparaciones.Remove(reparacion);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
