namespace Proyecto_EmilEncalada.Server.Services
{
    public class ReglaDecision
    {
        public string Nombre { get; set; } = string.Empty;
        public Func<decimal, decimal, decimal, bool> Condicion { get; set; } = null!;
        public string Resultado { get; set; } = string.Empty;
    }
}