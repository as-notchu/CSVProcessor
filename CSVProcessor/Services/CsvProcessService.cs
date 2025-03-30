using System.Globalization;
using CsvHelper;
using CSVProcessor.Enum;

using CSVProcessor.Models;
using CSVProcessor.Models.DTO;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using CsvContext = CSVProcessor.Database.CsvContext;
using Exception = System.Exception;

namespace CSVProcessor.Services;

public class CsvProcessService
{
    private readonly CsvContext _csvContext;

    private readonly ILogger<CsvProcessService> _logger;
    
    private readonly ActorService _actorResolver;
    public CsvProcessService(CsvContext csvContext, ILogger<CsvProcessService> logger, ActorService actorResolver)
    {
        _csvContext = csvContext;
        _logger = logger;
        _actorResolver = actorResolver;
    }

    public async Task<ServiceResult<bool>> ProcessCsv(string filePath)
    {
        var streamReader = new StreamReader(filePath);
        
        var reader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
        
        var resultDynamic = reader.GetRecords<dynamic>().ToList();

        var result = new List<FilmCreateDTO>();
        foreach (var res in resultDynamic)
        {
            string str = res.Actors;
            
            result.Add(new FilmCreateDTO()
            {
                Title = res.Title,
                Budget = res.Budget,
                ReleaseDate = res.ReleaseDate,
                Actors = str.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
            });
        }
        
        if (result.Count == 0)
        {
            return ServiceResult<bool>.Fail(ServiceErrorCodes.CantParseData, $"Can't parse file");
        }
        
        List<FilmData> films = new List<FilmData>();

        var actorsDto = result.SelectMany(x => x.Actors).Distinct().ToList();

        var allActors = await _actorResolver.GetOrCreateActorsAsync(actorsDto);
        
        foreach (var data in result)
        {
            _logger.LogInformation($"Film: {data.Title}, Budget: {data.Budget}, ReleaseDate: {data.ReleaseDate}");

            var film = new FilmData(data);

            foreach (var actorName in data.Actors.Distinct())
            {
                film.Actors.Add(allActors[actorName]);
            }

            films.Add(film);
        }
        
        films = films
            .GroupBy(f => f.Title, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        var filmTitles = films.Select(f => f.Title).Distinct().ToList();

        var dbFilms = await _csvContext.Films
            .Where(f => filmTitles.Contains(f.Title))
            .Include(f => f.Actors)
            .ToListAsync();
        
        var filmDict = dbFilms.ToDictionary(f => f.Title, StringComparer.OrdinalIgnoreCase);

        
        foreach (var film in films)
        {
            
            filmDict.TryGetValue(film.Title, out var dbFilm);
            
            if (dbFilm != null)
            {
                dbFilm.Actors.Clear();
                foreach (var actor in film.Actors)
                {
                    dbFilm.Actors.Add(actor);
                }
                dbFilm.ReleaseDate = film.ReleaseDate;
                dbFilm.Budget = film.Budget;
                continue;
            }
            _csvContext.Films.Add(film);
        }
        
        await _csvContext.SaveChangesAsync();
        
        return ServiceResult<bool>.Ok(true);
        
    }

    public async Task<ServiceResult<FileStream>> GetCsvFileFromDb()
    {
        
        var films = await _csvContext.Films.AsNoTracking().ToListAsync();

        if (films.Count == 0)
        {
            return ServiceResult<FileStream>.Fail(ServiceErrorCodes.Unknown, $"Cant get films from db or db is empty");
        }
        
        List<FilmResponseDTO> filmDTOs = new List<FilmResponseDTO>();
        
        foreach (var film in films)
        {
            filmDTOs.Add(new FilmResponseDTO(film));
        }
        
        var filePath = Directory.GetCurrentDirectory() + "/films.csv";
        
        using (var writer = new StreamWriter(filePath))
            
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            try
            {
                await csv.WriteRecordsAsync(filmDTOs);
            }
            catch (Exception e)
            {
                return ServiceResult<FileStream>.Fail(ServiceErrorCodes.SaveFailed, e.Message);
            }
            
        }

        return ServiceResult<FileStream>.Ok(new FileStream(filePath, FileMode.Open, FileAccess.Read));

    }

    
}