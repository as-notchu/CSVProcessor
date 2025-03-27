using CSVProcessor.Database;
using CSVProcessor.Enum;
using CSVProcessor.Interfaces;
using CSVProcessor.Models;
using CSVProcessor.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace CSVProcessor.Services;

public class DataService
{
    private readonly CsvContext _csvContext;

    private readonly ILogger<DataService> _logger;
    
    private readonly IActorResolver _actorResolver;
    

    public DataService(CsvContext csvContext, ILogger<DataService> logger, IActorResolver actorResolver)
    {
        _csvContext = csvContext;
        _logger = logger;
        _actorResolver = actorResolver;
    }
    
    
    
    public async Task<ServiceResult<List<FilmData>>> GetFilms()
    {
        List<FilmData> films = new List<FilmData>();
        
        try
        {
            films = await _csvContext.Films.AsNoTracking().ToListAsync();
        }
        catch (Exception e)
        {
            return ServiceResult<List<FilmData>>.Fail(ServiceErrorCodes.Unknown, e.Message);
        }
        
        return ServiceResult<List<FilmData>>.Ok(films);

    }

    public async Task<ServiceResult<Guid>> AddFilm(FilmDTO filmData)
    {
        if (await _csvContext.Films.AnyAsync(x => x.Title == filmData.Title))
        {
            return ServiceResult<Guid>.Fail(
                ServiceErrorCodes.Duplicate,
                $"Film with title '{filmData.Title}' already exists.");
        }

        var actorTitles = filmData.Actors.Distinct().ToList();

        var allActors = await _actorResolver.GetOrCreateActorsAsync(actorTitles);

        var film = new FilmData(filmData);
        foreach (var actor in allActors)
        {
            film.Actors.Add(actor.Value);
        }
        
        try
        {
            await _csvContext.Films.AddAsync(film);
            
            await _csvContext.SaveChangesAsync();
            
            return ServiceResult<Guid>.Ok(film.Id);
        }
        
        catch (Exception e)
        {
            return ServiceResult<Guid>.Fail(ServiceErrorCodes.SaveFailed, e.Message);
        }
        
    }
    
    public async Task<ServiceResult<FilmData>> GetFilmById(Guid id, bool tracking = false, bool includeActors = false)
    {
        var query = _csvContext.Films.AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();
        if (!includeActors)
        {
            query = query.Include(x => x.Actors);
        }
        FilmData? filmData;
        try
        {
            filmData = await query
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        catch (Exception e)
        {
            return ServiceResult<FilmData>.Fail(ServiceErrorCodes.Unknown, e.Message);
        }
        

        if (filmData == null)
        {
            return ServiceResult<FilmData>.Fail(ServiceErrorCodes.NotFound, $"Film with id {id} was not found.");
        }
        
        return ServiceResult<FilmData>.Ok(filmData);
        
    }

    public async Task<ServiceResult<Unit>> DeleteFilm(Guid id)
    {
        var filmResult = await GetFilmById(id);
        if (!filmResult.Success)
        {
            return ServiceResult<Unit>.Fail(filmResult.ErrorCode, filmResult.Error!);
        }

        var film = filmResult.Data;
        
        if (film == null) return ServiceResult<Unit>.Fail(
            ServiceErrorCodes.NotFound, 
            $"Film with id: {id} does not exist.");

        try
        {
            _csvContext.Films.Remove(film);
        
            await _csvContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return ServiceResult<Unit>.Fail(ServiceErrorCodes.SaveFailed, e.Message);
        }
        

        return ServiceResult<Unit>.Ok(new Unit());

    }

    public async Task<ServiceResult<FilmData>> UpdateFilm(FilmDTO filmDto, Guid id)
    {
        var film = new FilmData(filmDto);
        
        var filmResult = await GetFilmById(id, true, true);

        if (!filmResult.Success)
        {
            return ServiceResult<FilmData>.Fail(filmResult.ErrorCode, filmResult.Error!);
        }
        
        var entity = filmResult.Data!;
        
        entity.Title = film.Title;
        
        entity.ReleaseDate = film.ReleaseDate;
        
        entity.Budget = film.Budget;
        
        entity.Actors.Clear();

        var actorTitles = film.Actors.Select(x => x.Name).Distinct().ToList();
        
        var allActors = await _actorResolver.GetOrCreateActorsAsync(actorTitles);
        
        foreach (var actor in allActors.Values)
        {
            entity.Actors.Add(actor);
        }
        
        try
        {
            await _csvContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return ServiceResult<FilmData>.Fail(ServiceErrorCodes.SaveFailed, e.Message);
        }
        
        return ServiceResult<FilmData>.Ok(entity);

    }
    
 }