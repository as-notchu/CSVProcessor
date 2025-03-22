using CSVProcessor.Services;
using Microsoft.AspNetCore.Mvc;

namespace CSVProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class CsvController : ControllerBase
{
    private readonly CsvProcessService _csvProcessService;

    public CsvController(CsvProcessService csvProcessService)
    {
        _csvProcessService = csvProcessService;
    }
 
    [HttpPost("process")]
    public async Task<IActionResult> ProcessCsv(IFormFile file)
    {
        var filePath = Path.GetTempFileName();
        
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        await _csvProcessService.ReadCsv(filePath);
        
        return Ok("CSV processed");
    }

        [HttpGet("getcsv")]
        public async Task<IActionResult> GetCsv()
        {
            var fileStream = await _csvProcessService.GetCsvFileFromDb();
            
            return File(fileStream, "text/csv", "films.csv");
        }
        
        
        
    
}