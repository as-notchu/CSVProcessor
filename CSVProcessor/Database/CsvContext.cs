using CSVProcessor.Models;
using Microsoft.EntityFrameworkCore;

namespace CSVProcessor.Database;

public class CsvContext : DbContext
{

    public DbSet<FilmData> Films { get; set; }
    
    public DbSet<Actor> Actors { get; set; }

    
    public CsvContext(DbContextOptions<CsvContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FilmData>()
            .HasKey(m => m.Id);
        
        modelBuilder.Entity<FilmData>()
            .HasIndex(m => m.Title);

        modelBuilder.Entity<Actor>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<Actor>()
            .HasIndex(a => a.Name)
            .IsUnique(true);
        
        modelBuilder.Entity<FilmData>()
            .HasMany(x => x.Actors)
            .WithMany(x => x.Films)
            .UsingEntity(x => x.ToTable("FilmActors"));
            
        
        base.OnModelCreating(modelBuilder);
    }
    
}