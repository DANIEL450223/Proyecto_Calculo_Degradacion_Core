using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_EmilEncalada.Server.Data;
using Proyecto_EmilEncalada.Server.Models;
using Proyecto_EmilEncalada.Server.Helpers;

namespace Proyecto_EmilEncalada.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartamentosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DepartamentosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Departamento>>> GetDepartamentos()
        {
            return await _context.Departamentos
                .OrderBy(d => d.Nombre)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Departamento>> GetDepartamento(int id)
        {
            var departamento = await _context.Departamentos.FindAsync(id);

            if (departamento == null)
                return NotFound(new { mensaje = "Departamento no encontrado." });

            return departamento;
        }

        [HttpPost]
        public async Task<ActionResult<Departamento>> PostDepartamento(Departamento departamento)
        {
            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            if (string.IsNullOrWhiteSpace(departamento.Nombre))
                return BadRequest(new { mensaje = "El nombre del departamento es obligatorio." });

            if (departamento.PresupuestoAsignado <= 0)
                return BadRequest(new { mensaje = "El presupuesto asignado debe ser mayor a cero." });

            _context.Departamentos.Add(departamento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDepartamento), new { id = departamento.Id }, departamento);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartamento(int id, Departamento departamento)
        {
            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            if (id != departamento.Id)
                return BadRequest(new { mensaje = "El id no coincide." });

            if (string.IsNullOrWhiteSpace(departamento.Nombre))
                return BadRequest(new { mensaje = "El nombre del departamento es obligatorio." });

            if (departamento.PresupuestoAsignado <= 0)
                return BadRequest(new { mensaje = "El presupuesto asignado debe ser mayor a cero." });

            var existeDepartamento = await _context.Departamentos.AnyAsync(d => d.Id == id);

            if (!existeDepartamento)
                return NotFound(new { mensaje = "Departamento no encontrado." });

            _context.Entry(departamento).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartamento(int id)
        {
            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            var departamento = await _context.Departamentos.FindAsync(id);

            if (departamento == null)
                return NotFound(new { mensaje = "Departamento no encontrado." });

            var tieneEquipos = await _context.Equipos.AnyAsync(e => e.DepartamentoId == id);

            if (tieneEquipos)
                return BadRequest(new { mensaje = "No se puede eliminar el departamento porque tiene equipos asociados." });

            _context.Departamentos.Remove(departamento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}