using System.Globalization;
using CsvHelper;
using CSVProcessor.Models;
using EFCore.BulkExtensions;
using CsvContext = CSVProcessor.Database.CsvContext;

namespace CSVProcessor.Services;

public class CsvProcessService
{
    private readonly CsvContext _csvContext;

    private readonly ILogger<CsvProcessService> _logger;
    public CsvProcessService(CsvContext csvContext, ILogger<CsvProcessService> logger)
    {
        _csvContext = csvContext;
        _logger = logger;
    }

    public async Task ReadCsv(string filePath)
    {
        var streamReader = new StreamReader(filePath);
        
        var reader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
        
        var result = reader.GetRecords<DataFromCsv>().ToList();
        
        List<FilmData> films = new List<FilmData>();

        foreach (var data in result)
        {
            _logger.LogInformation($"Film: {data.Title}, Budget: {data.Budget}, ReleaseDate: {data.ReleaseDate}");
            
            films.Add(new FilmData(data));
        }
        
        await _csvContext.BulkInsertAsync(films);
    }
}