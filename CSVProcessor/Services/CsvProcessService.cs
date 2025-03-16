using CSVProcessor.Database;

namespace CSVProcessor.Services;

public class CsvProcessService
{
    private readonly CsvContext _csvContext;

    public CsvProcessService(CsvContext csvContext)
    {
        _csvContext = csvContext;
    }

    public async Task ReadCsv(string filePath)
    {
        
    }
}