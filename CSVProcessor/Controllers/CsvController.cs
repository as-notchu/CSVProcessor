using CSVProcessor.Helpers;
using CSVProcessor.Services;
using Microsoft.AspNetCore.Mvc;

namespace CSVProcessor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CsvController : ControllerBase
{
    private readonly CsvProcessService _csvService;

    private readonly ILogger<CsvController> _logger;
    public CsvController(CsvProcessService csvService, ILogger<CsvController> logger)
    {
        _csvService = csvService;
        _logger = logger;
    }
 
    [HttpPost]
    public async Task<IActionResult> ProcessCsv(IFormFile file)
    {
        var filePath = Path.GetTempFileName();
        
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var result = await _csvService.ReadCsv(filePath);

        if (!result.Success)
        {
            return result.ToActionResult(_logger);
        }
        
        return Ok("CSV processed");
    }

    [HttpGet]
    public async Task<IActionResult> GetCsv()
    {
        var result = await _csvService.GetCsvFileFromDb();

        if (!result.Success || result.Data == null)
        {
            return result.ToActionResult(_logger);
        }
        
        return File(result.Data, "text/csv", "films.csv");
    }

       
        
    
}