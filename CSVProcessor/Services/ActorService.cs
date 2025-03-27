using CSVProcessor.Database;
using CSVProcessor.Interfaces;
using CSVProcessor.Models;
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
}