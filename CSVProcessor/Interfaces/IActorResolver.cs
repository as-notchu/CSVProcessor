using CSVProcessor.Models;

namespace CSVProcessor.Interfaces;

public interface IActorResolver
{
    public Task<Dictionary<string, Actor>> GetOrCreateActorsAsync(List<string> incomingActorNames);
}