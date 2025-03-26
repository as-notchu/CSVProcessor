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
    
    
    
    public async Task<ServiceResult<IEnumerable<FilmData>>> GetFilms()
    {
        List<FilmData> films = new List<FilmData>();
        
        try
        {
            films = await _csvContext.Films.AsNoTracking().ToListAsync();
        }
        catch (Exception e)
        {
            return ServiceResult<IEnumerable<FilmData>>.Fail(ServiceErrorCodes.Unknown, e.Message);
        }
        
        return ServiceResult<IEnumerable<FilmData>>.Ok(films);

    }

    public async Task<ServiceResult<Guid>> AddFilm(FilmDTO filmData)
    {
        var film = new FilmData(filmData);

        if (await _csvContext.Films.AnyAsync(x => x.Title == film.Title))
        {
            return ServiceResult<Guid>.Fail(
                ServiceErrorCodes.Duplicate,
                $"Film with title '{film.Title}' already exists.");
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

    public async Task<ServiceResult<FilmDataDTO>> GetFilmDtoById(Guid id, bool tracking = false)
    {
        var query = _csvContext.Films.AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();
        FilmData? film;
        try
        {
            film = await query
                .Include(x => x.Actors)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        catch (Exception e)
        {
            return ServiceResult<FilmDataDTO>.Fail(ServiceErrorCodes.Unknown, e.Message);
        }


        if (film == null)
        {
            return ServiceResult<FilmDataDTO>.Fail(ServiceErrorCodes.NotFound, $"Film not found");
        }
        
        FilmDataDTO filmDto = new FilmDataDTO(film);
        
        return ServiceResult<FilmDataDTO>.Ok(filmDto);
    }
    public async Task<ServiceResult<FilmData>> GetFilmById(Guid id, bool tracking = false, bool dto = false)
    {
        var query = _csvContext.Films.AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();
        
        FilmData? filmData;
        try
        {
            filmData = await _csvContext.Films
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
        
        var filmResult = await GetFilmById(id, true);

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
        
        List<Actor> actorsFromDatabase = await _csvContext.Actors
            .Where(x => actorTitles.Contains(x.Name))
            .ToListAsync();

        var actorsDict = actorsFromDatabase.ToDictionary(x => x.Name);

        var allActors = new Dictionary<string, Actor>();
        
        var actorsToAdd = new List<Actor>();
        
        foreach (var actorDto in actorTitles)
        {
            if (actorsDict.TryGetValue(actorDto, out var outActor))
            {
                allActors[actorDto] = outActor;
                continue;
            }
            var actor = new Actor(actorDto);
            allActors[actorDto] = actor;
            actorsToAdd.Add(actor);
        }
        _csvContext.Actors.AddRange(actorsToAdd);

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