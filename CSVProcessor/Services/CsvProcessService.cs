using System.Globalization;
using CsvHelper;
using CSVProcessor.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
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
        
        var result = reader.GetRecords<FilmDTO>().ToList();
        
        List<FilmData> films = new List<FilmData>();

        foreach (var data in result)
        {
            _logger.LogInformation($"Film: {data.Title}, Budget: {data.Budget}, ReleaseDate: {data.ReleaseDate}");
            
            films.Add(new FilmData(data));
        }
        
        await _csvContext.BulkInsertAsync(films);
    }

    public async Task<FileStream> GetCsvFileFromDb()
    {
        
        var films = await _csvContext.Films.AsNoTracking().ToListAsync();
        
        var filePath = Directory.GetCurrentDirectory() + "/films.csv";
        
        using (var writer = new StreamWriter(filePath))
            
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            await csv.WriteRecordsAsync(films);
        }

        return new FileStream(filePath, FileMode.Open, FileAccess.Read);

    }

    
}