using Proyecto_EmilEncalada.Server.Models;

namespace Proyecto_EmilEncalada.Server.Services
{
    public class DegradacionService
    {
        public object Calcular(Equipo equipo, List<Reparacion> reparaciones)
        {
            DateTime hoy = DateTime.Today;
            DateTime fechaCompra = equipo.FechaCompra.Date;

            double diasTranscurridos = (hoy - fechaCompra).TotalDays;
            if (diasTranscurridos < 0)
                diasTranscurridos = 0;

            decimal mesesTranscurridos = (decimal)(diasTranscurridos / 30.44);
            if (mesesTranscurridos < 0)
                mesesTranscurridos = 0;

            decimal porcentajeUso = 0;
            if (equipo.VidaUtilMeses > 0)
                porcentajeUso = (mesesTranscurridos / equipo.VidaUtilMeses) * 100;

            decimal costoReparaciones = 0;
            int cantidadReparaciones = 0;

            foreach (var reparacion in reparaciones)
            {
                costoReparaciones += reparacion.Costo;
                cantidadReparaciones++;
            }

            decimal factorTipo = 1.0m;
            decimal factorEstado = 1.0m;
            decimal factorReparaciones = 1.0m;

            string tipoEquipo = equipo.Tipo.Trim().ToLower();
            string estadoEquipo = equipo.Estado.Trim().ToLower();

            var factoresTipo = new List<(string Tipo, decimal Factor)>
            {
                ("laptop", 1.20m),
                ("portatil", 1.20m),
                ("escritorio", 1.00m),
                ("pc", 1.00m),
                ("impresora", 1.10m),
                ("servidor", 1.35m),
                ("televisor", 1.05m),
                ("electronico", 1.08m),
                ("general", 1.00m)
            };

            var factoresEstado = new List<(string Estado, decimal Factor)>
            {
                ("activo", 1.00m),
                ("regular", 1.10m),
                ("deteriorado", 1.25m),
                ("crítico", 1.40m),
                ("critico", 1.40m),
                ("danado", 1.40m),
                ("general", 1.00m)
            };

            var reglasReparacion = new List<(int MinCantidad, decimal Factor)>
            {
                (0, 1.00m),
                (1, 1.05m),
                (2, 1.10m),
                (3, 1.20m),
                (5, 1.30m)
            };

            bool tipoEncontrado = false;
            bool estadoEncontrado = false;

            foreach (var tipo in factoresTipo)
            {
                bool coincideTipo = tipoEquipo.Contains(tipo.Tipo) || tipo.Tipo == "general";

                if (coincideTipo && !tipoEncontrado)
                {
                    factorTipo = tipo.Factor;
                    tipoEncontrado = true;

                    foreach (var estado in factoresEstado)
                    {
                        bool coincideEstado = estadoEquipo.Contains(estado.Estado) || estado.Estado == "general";

                        if (coincideEstado && !estadoEncontrado)
                        {
                            factorEstado = estado.Factor;
                            estadoEncontrado = true;

                            foreach (var regla in reglasReparacion)
                            {
                                if (cantidadReparaciones >= regla.MinCantidad)
                                {
                                    factorReparaciones = regla.Factor;
                                }
                            }
                        }
                    }
                }
            }

            decimal degradacion = porcentajeUso * factorTipo * factorEstado * factorReparaciones;

            decimal porcentajeCostoReparacion = 0;
            if (equipo.CostoInicial > 0)
                porcentajeCostoReparacion = (costoReparaciones / equipo.CostoInicial) * 100;

            var tramosCosto = new List<(decimal Limite, decimal Extra)>
            {
                (10, 0),
                (20, 3),
                (35, 7),
                (50, 12),
                (1000, 18)
            };

            foreach (var tramo in tramosCosto)
            {
                if (porcentajeCostoReparacion <= tramo.Limite)
                {
                    degradacion += tramo.Extra;
                    break;
                }
            }

            if (degradacion > 100)
                degradacion = 100;

            if (degradacion < 0)
                degradacion = 0;

            decimal valorActualEstimado = equipo.CostoInicial * (1 - (degradacion / 100));
            if (valorActualEstimado < 0)
                valorActualEstimado = 0;

            decimal vidaRestanteMeses = equipo.VidaUtilMeses - mesesTranscurridos;
            if (vidaRestanteMeses < 0)
                vidaRestanteMeses = 0;

            string nivelDegradacion = "Baja";

            var niveles = new List<(decimal Limite, string Nombre)>
            {
                (40, "Baja"),
                (70, "Media"),
                (85, "Alta"),
                (101, "Crítica")
            };

            foreach (var nivel in niveles)
            {
                if (degradacion < nivel.Limite)
                {
                    nivelDegradacion = nivel.Nombre;
                    break;
                }
            }

            string recomendacion = "Mantener";

            var gruposDecision = new List<List<(Func<bool> Condicion, string Resultado)>>
            {
                new List<(Func<bool>, string)>
                {
                    (() => degradacion >= 85, "Reemplazar"),
                    (() => porcentajeCostoReparacion >= 50, "Reemplazar")
                },
                new List<(Func<bool>, string)>
                {
                    (() => degradacion >= 50, "Reparar"),
                    (() => cantidadReparaciones >= 3, "Revisar y Reparar")
                },
                new List<(Func<bool>, string)>
                {
                    (() => degradacion < 50, "Mantener")
                }
            };

            bool decisionTomada = false;

            foreach (var grupo in gruposDecision)
            {
                foreach (var regla in grupo)
                {
                    if (regla.Condicion() && !decisionTomada)
                    {
                        recomendacion = regla.Resultado;
                        decisionTomada = true;
                    }
                }
            }

            return new
            {
                Equipo = equipo.Nombre,
                Tipo = equipo.Tipo,
                Marca = equipo.Marca,
                Estado = equipo.Estado,
                AntiguedadMeses = Math.Round(mesesTranscurridos, 2),
                VidaUtilMeses = equipo.VidaUtilMeses,
                VidaRestanteMeses = Math.Round(vidaRestanteMeses, 2),
                CostoInicial = equipo.CostoInicial,
                CantidadReparaciones = cantidadReparaciones,
                CostoReparaciones = Math.Round(costoReparaciones, 2),
                PorcentajeCostoReparacion = Math.Round(porcentajeCostoReparacion, 2),
                DegradacionPorcentaje = Math.Round(degradacion, 2),
                NivelDegradacion = nivelDegradacion,
                ValorActualEstimado = Math.Round(valorActualEstimado, 2),
                Recomendacion = recomendacion
            };
        }
    }
}