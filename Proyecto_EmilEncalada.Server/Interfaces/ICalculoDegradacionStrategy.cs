using Proyecto_EmilEncalada.Server.Dtos;
using Proyecto_EmilEncalada.Server.Models;

namespace Proyecto_EmilEncalada.Server.Interfaces;

public interface ICalculoDegradacionStrategy
{
    ResultadoDegradacionDto Calcular(Equipo equipo, List<Reparacion> reparaciones);
}