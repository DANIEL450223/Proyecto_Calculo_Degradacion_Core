using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Proyecto_EmilEncalada.Server.Models
{
    public class Departamento
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Range(0.01, 100000000)]
        public decimal PresupuestoAsignado { get; set; }

        [JsonIgnore]
        public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
    }
}   