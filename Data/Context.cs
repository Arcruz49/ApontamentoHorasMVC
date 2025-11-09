using Microsoft.EntityFrameworkCore;
using ApontamentoHoras.Models;

namespace ApontamentoHoras.Data;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options) { }

    public DbSet<Usuario> usuario { get; set; }
    public DbSet<Apontamento> apontamento { get; set; }
    public DbSet<Cargo> cargo { get; set; }
    public DbSet<Turno> turno { get; set; }
    public DbSet<TurnoWeekday> turno_weekday { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .Property(u => u.dtCreation)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Apontamento>()
            .Property(a => a.dtApontamento)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Cargo>()
            .Property(a => a.dtCreation)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Turno>()
            .Property(a => a.dtCreation)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }

}



