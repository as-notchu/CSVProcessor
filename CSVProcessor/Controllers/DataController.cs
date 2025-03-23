using CSVProcessor.Models;
using CSVProcessor.Services;
using Microsoft.AspNetCore.Mvc;

namespace CSVProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase
{
    private readonly ILogger<DataController> _logger;
    
    private readonly DataService _dataService;
    

    public DataController(ILogger<DataController> logger, DataService dataService)
    {
        _logger = logger;
        _dataService = dataService;
    }
    
    [HttpGet("getfilms")]
    public async Task<IActionResult> GetFilms()
    {
        var films = await _dataService.GetFilms();
            
        return Ok(films);
    }

    [HttpGet("getfilm/{id}")]
    public async Task<IActionResult> GetFilm(Guid id)
    {
        var film = await _dataService.GetFilmById(id);

        if (film == null)
        {
            return NotFound($"Film with ID {id} not found");
        }
            
        return Ok(film);
    }

    [HttpDelete("deletefilm/{id}")]
    public async Task<IActionResult> DeleteFilm(Guid id)
    {

        if (await _dataService.DeleteFilm(id))
        {
            return Ok($"Film with ID {id} deleted");
        }       
        return NotFound($"Film with ID {id} not found");
        
    }

    [HttpPut("updatefilm/{id}")]
    public async Task<IActionResult> UpdateFilm(Guid id, [FromBody] FilmDTO filmDto)
    {
        
        FilmData filmData = new FilmData()
        {
            Id = id,
            Budget = filmDto.Budget,
            Title =filmDto.Title,
            ReleaseDate = filmDto.ReleaseDate
        };
         
        if(!await _dataService.UpdateFilm(filmData)) return NotFound($"Film with ID {id} not found");

        return NoContent();

    }


}