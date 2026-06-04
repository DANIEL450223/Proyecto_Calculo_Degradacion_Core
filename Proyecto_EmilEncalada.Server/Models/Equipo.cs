using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Proyecto_EmilEncalada.Server.Models
{
    public class Equipo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de equipo es obligatorio.")]
        [StringLength(50)]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La marca es obligatoria.")]
        [StringLength(50)]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de compra es obligatoria.")]
        [Column(TypeName = "date")]
        public DateTime FechaCompra { get; set; }

        [Range(0.01, 100000000, ErrorMessage = "El costo inicial debe ser mayor a cero.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CostoInicial { get; set; }

        [Range(1, 240, ErrorMessage = "La vida útil debe estar entre 1 y 240 meses.")]
        public int VidaUtilMeses { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [StringLength(50)]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un departamento.")]
        public int DepartamentoId { get; set; }

        public Departamento? Departamento { get; set; }

        [JsonIgnore]
        public ICollection<Reparacion> Reparaciones { get; set; } = new List<Reparacion>();
    }
}