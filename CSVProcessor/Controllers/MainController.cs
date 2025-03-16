using CSVProcessor.Services;
using Microsoft.AspNetCore.Mvc;

namespace CSVProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class MainController : ControllerBase
{
    private readonly CsvProcessService _csvProcessService;

    public MainController(CsvProcessService csvProcessService)
    {
        _csvProcessService = csvProcessService;
    }
 
    [HttpPost("process")]
    public async Task<IActionResult> ProcessCsv(IFormFile file)
    {
        var filePath = Path.GetTempFileName();
        
        var stream = new FileStream(filePath, FileMode.Create);
        
        await file.CopyToAsync(stream);

        await _csvProcessService.ReadCsv(filePath);
        
        return Ok("CSV processed");
    }
    
}