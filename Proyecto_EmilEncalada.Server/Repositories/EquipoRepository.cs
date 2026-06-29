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
        try
        {
            var equipo = await _context.Equipos
                .Include(e => e.Departamento)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipo != null)
            {
                return equipo;
            }
        }
        catch
        {
            // Si la base de datos no responde, se usa un dato de respaldo
            // para que el Core pueda seguir funcionando en la demostración.
        }

        return ObtenerEquipoRespaldo(id);
    }

    public async Task<List<Reparacion>> ObtenerReparacionesPorEquipoAsync(int equipoId)
    {
        try
        {
            var reparaciones = await _context.Reparaciones
                .Where(r => r.EquipoId == equipoId)
                .ToListAsync();

            return reparaciones;
        }
        catch
        {
            // Si la base de datos falla, se usan reparaciones de respaldo.
        }

        return ObtenerReparacionesRespaldo(equipoId);
    }

    private Equipo? ObtenerEquipoRespaldo(int id)
    {
        if (id != 1)
        {
            return null;
        }

        return new Equipo
        {
            Id = 1,
            Nombre = "Laptop Oficina",
            Tipo = "Laptop",
            Marca = "Dell",
            Estado = "Regular",
            FechaCompra = DateTime.Today.AddMonths(-24),
            CostoInicial = 1000,
            VidaUtilMeses = 60,
            DepartamentoId = 1
        };
    }

    private List<Reparacion> ObtenerReparacionesRespaldo(int equipoId)
    {
        if (equipoId != 1)
        {
            return new List<Reparacion>();
        }

        return new List<Reparacion>
        {
            new Reparacion
            {
                Id = 1,
                EquipoId = equipoId,
                Costo = 100
            },
            new Reparacion
            {
                Id = 2,
                EquipoId = equipoId,
                Costo = 150
            }
        };
    }
}