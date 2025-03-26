using CSVProcessor.Database;
using CSVProcessor.Interfaces;
using CSVProcessor.Models;
using Microsoft.EntityFrameworkCore;

namespace CSVProcessor.Services;

public class ActorService(CsvContext _csvContext) : IActorResolver
{
    
    public async Task<List<Actor>> GetOrCreateActorsAsync(List<string> incomingActorNames)
    {
        throw new NotImplementedException();
    }
}