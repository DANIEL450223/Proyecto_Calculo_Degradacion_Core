using Microsoft.EntityFrameworkCore;
using Proyecto_EmilEncalada.Server.Models;

namespace Proyecto_EmilEncalada.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Reparacion> Reparaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>().Property(p => p.Precio).HasPrecision(10, 2);
            modelBuilder.Entity<Departamento>().Property(d => d.PresupuestoAsignado).HasPrecision(10, 2);
            modelBuilder.Entity<Equipo>().Property(e => e.CostoInicial).HasPrecision(10, 2);
            modelBuilder.Entity<Reparacion>().Property(r => r.Costo).HasPrecision(10, 2);

            modelBuilder.Entity<Equipo>()
                .HasOne(e => e.Departamento)
                .WithMany(d => d.Equipos)
                .HasForeignKey(e => e.DepartamentoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reparacion>()
                .HasOne(r => r.Equipo)
                .WithMany(e => e.Reparaciones)
                .HasForeignKey(r => r.EquipoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
