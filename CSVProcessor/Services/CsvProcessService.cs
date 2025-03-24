using System.Globalization;
using CsvHelper;
using CSVProcessor.Enum;
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

    public async Task<ServiceResult<bool>> ReadCsv(string filePath)
    {
        var streamReader = new StreamReader(filePath);
        
        var reader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
        
        var result = reader.GetRecords<FilmDTO>().ToList();
        
        if (result.Count == 0)
        {
            return ServiceResult<bool>.Fail(ServiceErrorCodes.CantParseData, $"Can't parse file");
        }
        
        List<FilmData> films = new List<FilmData>();

        foreach (var data in result)
        {
            _logger.LogInformation($"Film: {data.Title}, Budget: {data.Budget}, ReleaseDate: {data.ReleaseDate}");
            
            films.Add(new FilmData(data));
        }

        try
        {
            await _csvContext.BulkInsertOrUpdateAsync(films, new BulkConfig
            {
                UpdateByProperties = new List<string> { "Title" }
            });

        }
        catch (Exception e)
        {
            return ServiceResult<bool>.Fail(ServiceErrorCodes.SaveFailed, e.Message);
        }
        
        return ServiceResult<bool>.Ok(true);
        
    }

    public async Task<ServiceResult<FileStream>> GetCsvFileFromDb()
    {
        
        var films = await _csvContext.Films.AsNoTracking().ToListAsync();

        if (films.Count == 0)
        {
            return ServiceResult<FileStream>.Fail(ServiceErrorCodes.Unknown, $"Cant get films from db or db is empty");
        }
        
        var filePath = Directory.GetCurrentDirectory() + "/films.csv";
        
        using (var writer = new StreamWriter(filePath))
            
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            try
            {
                await csv.WriteRecordsAsync(films);
            }
            catch (Exception e)
            {
                return ServiceResult<FileStream>.Fail(ServiceErrorCodes.SaveFailed, e.Message);
            }
            
        }

        return ServiceResult<FileStream>.Ok(new FileStream(filePath, FileMode.Open, FileAccess.Read));

    }

    
}