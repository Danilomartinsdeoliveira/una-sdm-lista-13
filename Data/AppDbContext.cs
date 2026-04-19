using AmericanAirlinesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AmericanAirlinesApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Aeronave> Aeronaves { get; set; }
        public DbSet<Tripulante> Tripulantes { get; set; }
        public DbSet<Voo> Voos { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Voo>()
                .HasOne(v => v.Aeronave)
                .WithMany(a => a.Voos)
                .HasForeignKey(v => v.AeronaveId);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Voo)
                .WithMany(v => v.Reservas)
                .HasForeignKey(r => r.VooId);
        }
    }
}
