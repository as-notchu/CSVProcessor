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

    public async Task<ServiceResult<Actor>> GetActor(string name, bool tracking = false, bool includeFilms = false)
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

        Actor? actor;

        try
        {
            actor = await query
                .FirstOrDefaultAsync(x => x.Name == name);
        }
        catch (Exception e)
        {
            return ServiceResult<Actor>.Fail(ServiceErrorCodes.NotFound, $"Actor {name} not found");
        }

        if (actor == null)
        {
            return ServiceResult<Actor>.Fail(ServiceErrorCodes.NotFound, $"Actor {name} not found");
        }

        return ServiceResult<Actor>.Ok(actor);

    }

    public async Task<ServiceResult<Unit>> CreateActor(ActorRequestDTO actorRequestData)
    {
        var actorDb = await GetActor(actorRequestData.Name, true);

        if (actorDb.Success)
        {
            return ServiceResult<Unit>.Fail(ServiceErrorCodes.Duplicate, $"Actor {actorRequestData.Name} found");
        }

        var actor = new Actor(actorRequestData.Name);
        
        var filmTitles = actorRequestData.FilmNames.Distinct().ToList();

        if (filmTitles.Count > 0)
        {
            var films = await _csvContext.Films
                .Where(x => filmTitles.Contains(x.Title))
                .ToListAsync();

            actor.Films.AddRange(films);
        }

        _csvContext.Actors.Add(actor);
        await _csvContext.SaveChangesAsync();

        return ServiceResult<Unit>.Ok(Unit.Value);
    }

    /*public async Task<ServiceResult<Actor>> UpdateActor(ActorRequestDTO dto)
    {
        Actor? actor;

        switch (dto.Id)
        {
            case not null:
                actor = await _csvContext.Actors.FirstOrDefaultAsync(x => x.Id == dto.Id);
                break;
            default:
                actor = await _csvContext.Actors.FirstOrDefaultAsync(x => x.Name == dto.Name);
                break;
        }

        if (actor == null)
        {
            return ServiceResult<Actor>.Fail(ServiceErrorCodes.NotFound, $"Actor {dto.Name} not found");
        }
        
        var filmTitles = dto.FilmNames.Distinct().ToList();

        var films = await _csvContext.Films
            .Where(x => filmTitles.Contains(x.Title))
            .ToListAsync();

        var allFilms = new List<FilmData>();

        foreach (var film in films)
        {
            
        }

    } */
}