using CSVProcessor.Database;
using CSVProcessor.Enum;
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

    public async Task<ServiceResult<Guid>> AddFilm(FilmDTO filmData)
    {
        var film = new FilmData(filmData);

        if (await _csvContext.Films.AnyAsync(x => x.Id == film.Id))
        {
            return ServiceResult<Guid>.Fail(
                DataServiceErrorCode.DuplicateId, 
                $"Film with id: {film.Id} already exists.");
        }
        
        try
        {
            await _csvContext.Films.AddAsync(film);
            
            await _csvContext.SaveChangesAsync();
            
            return ServiceResult<Guid>.Ok(film.Id);
        }
        catch (Exception e)
        {
            return ServiceResult<Guid>.Fail(DataServiceErrorCode.SaveFailed, e.Message);
        }
    }


    public async Task<FilmData?> GetFilmById(Guid id, bool tracking = false)
    {
        var query = _csvContext.Films.AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<ServiceResult<Unit>> DeleteFilm(Guid id)
    {
        var film = await GetFilmById(id);
        
        if (film == null) return ServiceResult<Unit>.Fail(
            DataServiceErrorCode.NotFound, 
            $"Film with id: {id} does not exist.");

        try
        {
            _csvContext.Films.Remove(film);
        
            await _csvContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return ServiceResult<Unit>.Fail(DataServiceErrorCode.SaveFailed, e.Message);
        }
        

        return ServiceResult<Unit>.Ok(new Unit());

    }

    public async Task<ServiceResult<FilmData>> UpdateFilm(FilmData film)
    {
        var entity = await GetFilmById(film.Id, true);
        
        if (entity is null) return ServiceResult<FilmData>.Fail(DataServiceErrorCode.NotFound, "Film not found.");

        entity.Title = film.Title;
        
        entity.ReleaseDate = film.ReleaseDate;
        
        entity.Budget = film.Budget;

        try
        {
            await _csvContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return ServiceResult<FilmData>.Fail(DataServiceErrorCode.SaveFailed, e.Message);
        }
        
        return ServiceResult<FilmData>.Ok(film);

    }
 }