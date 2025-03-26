using CSVProcessor.Models;

namespace CSVProcessor.Interfaces;

public interface IActorResolver
{
    public Task<List<Actor>> GetOrCreateActorsAsync(List<string> incomingActorNames);
}