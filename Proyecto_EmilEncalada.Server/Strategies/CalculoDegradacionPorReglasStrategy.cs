using Proyecto_EmilEncalada.Server.Dtos;
using Proyecto_EmilEncalada.Server.Interfaces;
using Proyecto_EmilEncalada.Server.Models;

namespace Proyecto_EmilEncalada.Server.Strategies;

public class CalculoDegradacionPorReglasStrategy : ICalculoDegradacionStrategy
{
    public ResultadoDegradacionDto Calcular(Equipo equipo, List<Reparacion> reparaciones)
    {
        DateTime hoy = DateTime.Today;

        double diasTranscurridos = (hoy - equipo.FechaCompra).TotalDays;
        decimal mesesTranscurridos = (decimal)(diasTranscurridos / 30.44);

        if (mesesTranscurridos < 0)
        {
            mesesTranscurridos = 0;
        }

        decimal porcentajeUso = 0;

        if (equipo.VidaUtilMeses > 0)
        {
            porcentajeUso = (mesesTranscurridos / equipo.VidaUtilMeses) * 100;
        }

        decimal factorTipo = ObtenerFactorTipo(equipo.Tipo);
        decimal factorEstado = ObtenerFactorEstado(equipo.Estado);
        decimal factorReparaciones = ObtenerFactorReparaciones(reparaciones.Count);

        decimal costoReparaciones = reparaciones.Sum(r => r.Costo);
        decimal porcentajeCostoReparacion = 0;

        if (equipo.CostoInicial > 0)
        {
            porcentajeCostoReparacion = (costoReparaciones / equipo.CostoInicial) * 100;
        }

        decimal extraCostoReparacion = ObtenerExtraPorCostoReparacion(porcentajeCostoReparacion);

        decimal degradacion = porcentajeUso * factorTipo * factorEstado * factorReparaciones;
        degradacion += extraCostoReparacion;

        if (degradacion > 100)
        {
            degradacion = 100;
        }

        if (degradacion < 0)
        {
            degradacion = 0;
        }

        decimal valorActualEstimado = equipo.CostoInicial * (1 - (degradacion / 100));

        decimal vidaRestanteMeses = equipo.VidaUtilMeses - mesesTranscurridos;

        if (vidaRestanteMeses < 0)
        {
            vidaRestanteMeses = 0;
        }

        string nivel = ObtenerNivelDegradacion(degradacion);
        string recomendacion = ObtenerRecomendacion(degradacion, porcentajeCostoReparacion, reparaciones.Count);

        return new ResultadoDegradacionDto
        {
            Equipo = equipo.Nombre,
            Tipo = equipo.Tipo,
            Marca = equipo.Marca,
            Estado = equipo.Estado,
            AntiguedadMeses = Math.Round(mesesTranscurridos, 2),
            VidaUtilMeses = equipo.VidaUtilMeses,
            VidaRestanteMeses = Math.Round(vidaRestanteMeses, 2),
            CostoInicial = equipo.CostoInicial,
            CantidadReparaciones = reparaciones.Count,
            CostoReparaciones = costoReparaciones,
            PorcentajeCostoReparacion = Math.Round(porcentajeCostoReparacion, 2),
            DegradacionPorcentaje = Math.Round(degradacion, 2),
            NivelDegradacion = nivel,
            ValorActualEstimado = Math.Round(valorActualEstimado, 2),
            Recomendacion = recomendacion
        };
    }

    private decimal ObtenerFactorTipo(string tipo)
    {
        tipo = tipo.ToLower();

        if (tipo.Contains("laptop") || tipo.Contains("portatil"))
            return 1.20m;

        if (tipo.Contains("servidor"))
            return 1.35m;

        if (tipo.Contains("impresora"))
            return 1.10m;

        if (tipo.Contains("televisor"))
            return 1.05m;

        if (tipo.Contains("electronico"))
            return 1.08m;

        return 1.00m;
    }

    private decimal ObtenerFactorEstado(string estado)
    {
        estado = estado.ToLower();

        if (estado.Contains("regular"))
            return 1.10m;

        if (estado.Contains("deteriorado"))
            return 1.25m;

        if (estado.Contains("critico") || estado.Contains("crítico") || estado.Contains("danado") || estado.Contains("dañado"))
            return 1.40m;

        return 1.00m;
    }

    private decimal ObtenerFactorReparaciones(int cantidadReparaciones)
    {
        if (cantidadReparaciones >= 5)
            return 1.30m;

        if (cantidadReparaciones >= 3)
            return 1.20m;

        if (cantidadReparaciones >= 2)
            return 1.10m;

        if (cantidadReparaciones >= 1)
            return 1.05m;

        return 1.00m;
    }

    private decimal ObtenerExtraPorCostoReparacion(decimal porcentajeCosto)
    {
        if (porcentajeCosto > 50)
            return 18;

        if (porcentajeCosto > 35)
            return 12;

        if (porcentajeCosto > 20)
            return 7;

        if (porcentajeCosto > 10)
            return 3;

        return 0;
    }

    private string ObtenerNivelDegradacion(decimal degradacion)
    {
        if (degradacion >= 85)
            return "Crítica";

        if (degradacion >= 70)
            return "Alta";

        if (degradacion >= 40)
            return "Media";

        return "Baja";
    }

    private string ObtenerRecomendacion(decimal degradacion, decimal porcentajeCostoReparacion, int cantidadReparaciones)
    {
        if (degradacion >= 85 || porcentajeCostoReparacion > 50)
            return "Se recomienda reemplazar el equipo.";

        if (degradacion >= 70 || cantidadReparaciones >= 3)
            return "Se recomienda evaluar reemplazo o reparación mayor.";

        if (degradacion >= 40)
            return "Se recomienda dar seguimiento y planificar mantenimiento.";

        return "El equipo puede mantenerse en operación.";
    }
}