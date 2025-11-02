using Microsoft.EntityFrameworkCore;
using ApontamentoHoras.Models;

namespace ApontamentoHoras.Data;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options) { }

    public DbSet<Usuario> usuario { get; set; }
    public DbSet<Apontamento> apontamento { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .Property(u => u.dtCreation)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Apontamento>()
            .Property(a => a.dtApontamento)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }

}



