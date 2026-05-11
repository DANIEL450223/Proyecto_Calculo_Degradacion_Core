using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_EmilEncalada.Server.Data;
using Proyecto_EmilEncalada.Server.Models;

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
        public async Task<ActionResult<IEnumerable<Equipo>>> GetEquipos() =>
            await _context.Equipos.Include(e => e.Departamento).OrderBy(e => e.Nombre).ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Equipo>> GetEquipo(int id)
        {
            var equipo = await _context.Equipos.Include(e => e.Departamento).FirstOrDefaultAsync(e => e.Id == id);
            return equipo == null ? NotFound() : equipo;
        }

        [HttpPost]
        public async Task<ActionResult<Equipo>> PostEquipo(Equipo equipo)
        {
            if (equipo.FechaCompra > DateTime.Now)
                return BadRequest(new { mensaje = "La fecha de compra no puede ser futura." });

            if (equipo.CostoInicial <= 0)
                return BadRequest(new { mensaje = "El costo inicial debe ser mayor a cero." });

            var departamentoExiste = await _context.Departamentos.AnyAsync(d => d.Id == equipo.DepartamentoId);
            if (!departamentoExiste)
                return BadRequest(new { mensaje = "El departamento seleccionado no existe." });

            _context.Equipos.Add(equipo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEquipo), new { id = equipo.Id }, equipo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipo(int id, Equipo equipo)
        {
            if (id != equipo.Id)
                return BadRequest(new { mensaje = "El id no coincide." });

            if (equipo.FechaCompra > DateTime.Now)
                return BadRequest(new { mensaje = "La fecha de compra no puede ser futura." });

            if (equipo.CostoInicial <= 0)
                return BadRequest(new { mensaje = "El costo inicial debe ser mayor a cero." });

            var departamentoExiste = await _context.Departamentos.AnyAsync(d => d.Id == equipo.DepartamentoId);
            if (!departamentoExiste)
                return BadRequest(new { mensaje = "El departamento seleccionado no existe." });

            _context.Entry(equipo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipo(int id)
        {
            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null) return NotFound();
            _context.Equipos.Remove(equipo);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
