using CSVProcessor.Database;
using CSVProcessor.Enum;

using CSVProcessor.Models;
using CSVProcessor.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace CSVProcessor.Services;

public class FilmService
{
    private readonly CsvContext _csvContext;

    private readonly ILogger<FilmService> _logger;
    
    private readonly ActorService _actorResolver;
    

    public FilmService(CsvContext csvContext, ILogger<FilmService> logger, ActorService actorResolver)
    {
        _csvContext = csvContext;
        _logger = logger;
        _actorResolver = actorResolver;
    }
    
    
    
    public async Task<ServiceResult<List<FilmData>>> GetFilms()
    {
        List<FilmData> films;
        
        try
        {
            films = await _csvContext.Films.AsNoTracking()
                .Include(x=> x.Actors)
                .ToListAsync();
        }
        catch (Exception e)
        {
            return ServiceResult<List<FilmData>>.Fail(ServiceErrorCodes.Unknown, e.Message);
        }
        
        return ServiceResult<List<FilmData>>.Ok(films);

    }

    public async Task<ServiceResult<Guid>> AddFilm(FilmRequestDTO filmRequestData)
    {
        if (await _csvContext.Films.AnyAsync(x => x.Title == filmRequestData.Title))
        {
            return ServiceResult<Guid>.Fail(
                ServiceErrorCodes.Duplicate,
                $"Film with title '{filmRequestData.Title}' already exists.");
        }

        var actorTitles = filmRequestData.Actors.Distinct().ToList();

        var allActors = await _actorResolver.GetOrCreateActorsAsync(actorTitles);

        var film = new FilmData(filmRequestData);
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
        
        if (includeActors)
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

    public async Task<ServiceResult<FilmResponseDTO>> UpdateFilm(FilmRequestDTO filmRequestDto, Guid id)
    {
        var film = new FilmData(filmRequestDto);
        
        var filmResult = await GetFilmById(id, true, true);

        if (!filmResult.Success)
        {
            return ServiceResult<FilmResponseDTO>.Fail(filmResult.ErrorCode, filmResult.Error!);
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
            return ServiceResult<FilmResponseDTO>.Fail(ServiceErrorCodes.SaveFailed, e.Message);
        }
        
        return ServiceResult<FilmResponseDTO>.Ok(new FilmResponseDTO(entity));

    }

   public async Task<ServiceResult<List<FilmResponseDTO>>> FindFilmsInRange(long min, long max, bool includeActors = false)
   {

       List<FilmData> films;
       var query = _csvContext.Films.AsQueryable();

       if (includeActors)
       {
           query = query.Include(x => x.Actors);
       }
       
       try
       {
           films  = await query
               .AsNoTracking()
               .Where(x => x.Budget >= min && x.Budget <= max)
               .ToListAsync();
       }
       catch (Exception e)
       {
           return ServiceResult<List<FilmResponseDTO>>.Fail(ServiceErrorCodes.Unknown, e.Message);
       }
       
       List<FilmResponseDTO> filmResponseDTOs = new List<FilmResponseDTO>();

       foreach (var film in films)
       {
           filmResponseDTOs.Add(new FilmResponseDTO(film, includeActors));
       }
       
       return ServiceResult<List<FilmResponseDTO>>.Ok(filmResponseDTOs);
   } 
    
 }