using Proyecto_EmilEncalada.Server.Models;

namespace Proyecto_EmilEncalada.Server.Interfaces;

public interface IEquipoRepository
{
    Task<Equipo?> ObtenerEquipoPorIdAsync(int id);
    Task<List<Reparacion>> ObtenerReparacionesPorEquipoAsync(int equipoId);
}