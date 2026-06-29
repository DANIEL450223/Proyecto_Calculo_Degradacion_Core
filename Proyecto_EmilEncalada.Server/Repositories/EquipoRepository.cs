using Microsoft.EntityFrameworkCore;
using Proyecto_EmilEncalada.Server.Data;
using Proyecto_EmilEncalada.Server.Interfaces;
using Proyecto_EmilEncalada.Server.Models;

namespace Proyecto_EmilEncalada.Server.Repositories;

public class EquipoRepository : IEquipoRepository
{
    private readonly ApplicationDbContext _context;

    public EquipoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Equipo?> ObtenerEquipoPorIdAsync(int id)
    {
        return await _context.Equipos
            .Include(e => e.Departamento)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Reparacion>> ObtenerReparacionesPorEquipoAsync(int equipoId)
    {
        return await _context.Reparaciones
            .Where(r => r.EquipoId == equipoId)
            .ToListAsync();
    }
}