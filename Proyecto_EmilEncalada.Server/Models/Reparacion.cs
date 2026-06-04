using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_EmilEncalada.Server.Models
{
    public class Reparacion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de reparación es obligatoria.")]
        [Column(TypeName = "date")]
        public DateTime FechaReparacion { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(250)]
        public string Descripcion { get; set; } = string.Empty;

        [Range(0.01, 100000000, ErrorMessage = "El costo debe ser mayor a cero.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Costo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un equipo.")]
        public int EquipoId { get; set; }

        public Equipo? Equipo { get; set; }
    }
}
