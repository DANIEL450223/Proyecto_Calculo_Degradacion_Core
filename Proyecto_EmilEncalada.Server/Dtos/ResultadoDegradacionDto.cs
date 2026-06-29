namespace Proyecto_EmilEncalada.Server.Dtos;

public class ResultadoDegradacionDto
{
    public string Equipo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;

    public decimal AntiguedadMeses { get; set; }
    public int VidaUtilMeses { get; set; }
    public decimal VidaRestanteMeses { get; set; }

    public decimal CostoInicial { get; set; }
    public int CantidadReparaciones { get; set; }
    public decimal CostoReparaciones { get; set; }
    public decimal PorcentajeCostoReparacion { get; set; }

    public decimal DegradacionPorcentaje { get; set; }
    public string NivelDegradacion { get; set; } = string.Empty;
    public decimal ValorActualEstimado { get; set; }
    public string Recomendacion { get; set; } = string.Empty;
}