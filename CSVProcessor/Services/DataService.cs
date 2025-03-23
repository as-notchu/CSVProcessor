using CSVProcessor.Database;
using CSVProcessor.Models;
using Microsoft.EntityFrameworkCore;

namespace CSVProcessor.Services;

public class DataService
{
    private readonly CsvContext _csvContext;

    private readonly ILogger<DataService> _logger;

    public DataService(CsvContext csvContext, ILogger<DataService> logger)
    {
        _csvContext = csvContext;
        _logger = logger;
    }
    
    
    
    public async Task<IEnumerable<FilmData>> GetFilms()
    {
        
        var films = await _csvContext.Films.AsNoTracking().ToListAsync();
        
        return films;
        
    }

    public async Task<FilmData?> GetFilmById(Guid id)
    {
        var film = await _csvContext.Films.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        
        return film ?? null;
    }

    public async Task<bool> DeleteFilm(Guid id)
    {
        var film = await _csvContext.Films.FirstOrDefaultAsync(x => x.Id == id);
        
        if (film == null) return false;
        
        _csvContext.Films.Remove(film);
        
        await _csvContext.SaveChangesAsync();

        return true;

    }

    public async Task<bool> UpdateFilm(FilmData film)
    {
        var entity = await _csvContext.Films.FindAsync(film.Id);
        
        if (entity is null) return false;

        entity.Title = film.Title;
        
        entity.ReleaseDate = film.ReleaseDate;
        
        entity.Budget = film.Budget;

        await _csvContext.SaveChangesAsync();
        
        return true;

    }
 }