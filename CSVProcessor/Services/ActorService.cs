using CSVProcessor.Database;
using CSVProcessor.Enum;
using CSVProcessor.Interfaces;
using CSVProcessor.Models;
using CSVProcessor.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace CSVProcessor.Services;

public class ActorService(CsvContext _csvContext) : IActorResolver
{
    
    public async Task<Dictionary<string, Actor>> GetOrCreateActorsAsync(List<string> actorTitles)
    {
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
        return allActors;
    }

    public async Task<ServiceResult<List<ActorResponseDTO>>> GetAllActors(bool includeFilms = false)
    {
        List<Actor> actors = await _csvContext.Actors
            .AsNoTracking()
            .Include(x => x.Films)
            .ToListAsync();

        if (actors.Count == 0)
        {
            return ServiceResult<List<ActorResponseDTO>>.Fail(ServiceErrorCodes.NotFound, $"Actors not found");
        }
        
        List<ActorResponseDTO> actorsDto = new List<ActorResponseDTO>();

        foreach (var actor in actors)
        {
            actorsDto.Add(new ActorResponseDTO(actor, includeFilms));
        }
        
        return ServiceResult<List<ActorResponseDTO>>.Ok(actorsDto);

    }

    public async Task<ServiceResult<ActorResponseDTO>> GetActor(string name, bool tracking = false, bool includeFilms = false)
    {

        Actor? actor;

        try
        {
            actor = await PGetActor(name, tracking,  includeFilms);
        }
        catch (Exception e)
        {
            return ServiceResult<ActorResponseDTO>.Fail(ServiceErrorCodes.NotFound, $"Actor {name} not found");
        }

        if (actor == null)
        {
            return ServiceResult<ActorResponseDTO>.Fail(ServiceErrorCodes.NotFound, $"Actor {name} not found");
        }

        return ServiceResult<ActorResponseDTO>.Ok(new ActorResponseDTO(actor, includeFilms));

    }

    private async Task<Actor?> PGetActor(string name,  bool tracking = false, bool includeFilms = false)
    {
        var query = _csvContext.Actors.AsQueryable();

        if (tracking)
        {
            query.AsNoTracking();
        }

        if (includeFilms)
        {
            query = query.Include(x => x.Films);
        }

        Actor? actor = await query
            .FirstOrDefaultAsync(x => x.Name == name);
        
        return actor;
    }

    public async Task<ServiceResult<Unit>> CreateActor(ActorRequestDTO actorRequestData)
    {
        var actorDb = await GetActor(actorRequestData.Name, true);

        if (actorDb.Success)
        {
            return ServiceResult<Unit>.Fail(ServiceErrorCodes.Duplicate, $"Actor {actorRequestData.Name} found");
        }

        var actor = new Actor(actorRequestData.Name);
        
        var errors = new List<WarningsDetails>();
        
        var filmTitles = actorRequestData.FilmNames.Distinct().ToList();

        if (filmTitles.Count > 0)
        {
            var films = await _csvContext.Films
                .Where(x => filmTitles.Contains(x.Title))
                .ToListAsync();

            actor.Films.AddRange(films);

            foreach (var filmTitle in filmTitles)
            {
                if (!films.Exists(x => x.Title == filmTitle))
                {
                    errors.Add(new WarningsDetails(Operations.GetFilm, $"Film {filmTitle} not found"));
                }
            }
        }
        

        _csvContext.Actors.Add(actor);
        await _csvContext.SaveChangesAsync();

        return ServiceResult<Unit>.Ok(Unit.Value, errors);
    }

    public async Task<ServiceResult<ActorResponseDTO>> UpdateActorInfo(Guid id, string name)
    {
        var actor = await _csvContext.Actors.FirstOrDefaultAsync(x => x.Id == id);
        
        if (actor == null)
        {
            return ServiceResult<ActorResponseDTO>.Fail(ServiceErrorCodes.NotFound, $"Actor {name} not found");
        }
        
        actor.Name = name;

        try
        {
            await _csvContext.SaveChangesAsync();
            return ServiceResult<ActorResponseDTO>.Ok(new ActorResponseDTO(actor));
        }
        catch (Exception e)
        {
            return ServiceResult<ActorResponseDTO>.Fail(ServiceErrorCodes.SaveFailed, $"Could not save entity. {e.Message}");
        }
    }

    public async Task<ServiceResult<ActorResponseDTO>> AddActorFilms(ActorRequestDTO dto)
    {
        var actor = await PGetActor(dto.Name, true, true);

        if (actor == null)
        {
            return ServiceResult<ActorResponseDTO>.Fail(ServiceErrorCodes.NotFound, $"Actor {dto.Name} not found");
        }
        
        var films = await _csvContext.Films
            .Where(x => dto.FilmNames.Contains(x.Title))
            .ToDictionaryAsync(x => x.Title);

        foreach (var filmName in dto.FilmNames)
        {
            if (!films.ContainsKey(filmName))
            {
                return ServiceResult<ActorResponseDTO>.Fail(ServiceErrorCodes.NotFound, $"Actor {dto.Name} not found");
            }
        }
        
        actor.Films.AddRange(films.Values);

        try
        {
            await _csvContext.SaveChangesAsync();
            return ServiceResult<ActorResponseDTO>.Ok(new ActorResponseDTO(actor, true));
        }
        catch (Exception e)
        {
            return ServiceResult<ActorResponseDTO>.Fail(ServiceErrorCodes.SaveFailed, $"Could not save entity. {e.Message}");
        }
    }

    public async Task<ServiceResult<ActorResponseDTO>> RemoveActorsFilms(ActorRequestDTO dto)
    {
        var actor = await PGetActor(dto.Name, true, true);
        
        if (actor == null)
        {
            return ServiceResult<ActorResponseDTO>.Fail(ServiceErrorCodes.NotFound, $"Actor {dto.Name} not found");
        }
        
        foreach (var filmName in dto.FilmNames)
        {
            var film = actor.Films.FirstOrDefault(x => x.Title == filmName);
            
            if (film == null) continue;
            
            actor.Films.Remove(film);
        }

        try
        {
            await _csvContext.SaveChangesAsync();
            return ServiceResult<ActorResponseDTO>.Ok(new ActorResponseDTO(actor, true));
        }
        catch (Exception e)
        {
            return ServiceResult<ActorResponseDTO>.Fail(ServiceErrorCodes.SaveFailed, $"Could not save entity. {e.Message}");
        }
    }

    public async Task<ServiceResult<ActorResponseDTO>> RemoveActor(string name)
    {
        var actor = await PGetActor(name, true);
        
        if (actor == null) return ServiceResult<ActorResponseDTO>.Fail(ServiceErrorCodes.NotFound, "Actor not found");
        
        _csvContext.Actors.Remove(actor);

        try
        {
            await _csvContext.SaveChangesAsync();
            return ServiceResult<ActorResponseDTO>.Ok(new ActorResponseDTO(actor));
        }
        catch (Exception e)
        {
            return ServiceResult<ActorResponseDTO>.Fail(ServiceErrorCodes.SaveFailed, $"Could not save entity. {e.Message}");
        }
    }
}