using Proyecto_EmilEncalada.Server.Dtos;
using Proyecto_EmilEncalada.Server.Interfaces;

namespace Proyecto_EmilEncalada.Server.Services;

public class DegradacionService
{
    private readonly IEquipoRepository _equipoRepository;
    private readonly ICalculoDegradacionStrategy _calculoDegradacionStrategy;

    public DegradacionService(
        IEquipoRepository equipoRepository,
        ICalculoDegradacionStrategy calculoDegradacionStrategy)
    {
        _equipoRepository = equipoRepository;
        _calculoDegradacionStrategy = calculoDegradacionStrategy;
    }

    public async Task<ResultadoDegradacionDto?> CalcularDegradacionAsync(int equipoId)
    {
        var equipo = await _equipoRepository.ObtenerEquipoPorIdAsync(equipoId);

        if (equipo == null)
        {
            return null;
        }

        var reparaciones = await _equipoRepository.ObtenerReparacionesPorEquipoAsync(equipoId);

        return _calculoDegradacionStrategy.Calcular(equipo, reparaciones);
    }
}