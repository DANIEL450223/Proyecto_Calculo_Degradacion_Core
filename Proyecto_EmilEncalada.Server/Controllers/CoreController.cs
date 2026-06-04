using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_EmilEncalada.Server.Data;
using Proyecto_EmilEncalada.Server.Models;
using Proyecto_EmilEncalada.Server.Services;
using Proyecto_EmilEncalada.Server.Helpers;

namespace Proyecto_EmilEncalada.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DegradacionService _degradacionService;

        public CoreController(ApplicationDbContext context)
        {
            _context = context;
            _degradacionService = new DegradacionService();
        }

        [HttpGet("degradacion/{id}")]
        public async Task<IActionResult> CalcularDegradacion(int id)
        {
            if (!SeguridadHelper.EstaAutenticado(Request))
                return SeguridadHelper.DebeIniciarSesion();

            var equipo = await _context.Equipos
                .Include(e => e.Departamento)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipo == null)
                return NotFound(new { mensaje = "Equipo no encontrado." });

            var reparaciones = await _context.Reparaciones
                .Where(r => r.EquipoId == id)
                .ToListAsync();

            List<Reparacion> reparacionesEquipo = new List<Reparacion>();

            foreach (var reparacion in reparaciones)
            {
                reparacionesEquipo.Add(reparacion);
            }

            var resultado = _degradacionService.Calcular(equipo, reparacionesEquipo);

            return Ok(resultado);
        }
    }
}