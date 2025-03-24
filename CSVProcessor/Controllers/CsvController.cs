using CSVProcessor.Helpers;
using CSVProcessor.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
    [SwaggerOperation(
        Summary = "Upload films from CSV",
        Description = "Extracts all films from csv and saves it to DB"
        )]
    [ProducesResponseType(typeof(string),200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ProcessCsv(IFormFile file)
    {
        var filePath = Path.GetTempFileName();
        
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var result = await _csvService.ProcessCsv(filePath);

        if (!result.Success)
        {
            return result.ToActionResult(_logger);
        }
        
        return Ok("CSV processed");
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Download films to csv from database",
        Description = "Extracts all films from database to csv file"
        )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(500)]
    [ProducesResponseType(400)]
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