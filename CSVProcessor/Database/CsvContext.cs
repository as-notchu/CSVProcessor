using CSVProcessor.Models;
using Microsoft.EntityFrameworkCore;

namespace CSVProcessor.Database;

public class CsvContext : DbContext
{

    public DbSet<FilmData> Films { get; set; }
    
    
    public CsvContext(DbContextOptions<CsvContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FilmData>()
            .HasKey(m => m.Id);

        base.OnModelCreating(modelBuilder);
    }
    
}