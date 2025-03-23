using CSVProcessor.Enum;
using CSVProcessor.Helpers;
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

        var result = await _dataService.DeleteFilm(id);

        return result.ToActionResult(_logger);

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
         
        var result = await _dataService.UpdateFilm(filmData);

        if (!result.Success)
        {
            return result.ToActionResult(_logger);
        }

        return CreatedAtAction(nameof(GetFilm), new { id = result.Data.Id }, result.Data);

    }

    [HttpPost("addfilm")]
    public async Task<IActionResult> AddFilm([FromBody] FilmDTO filmDto)
    {
        var result = await _dataService.AddFilm(filmDto);

        if (!result.Success)
        {
            return result.ToActionResult(_logger);
        }

        return CreatedAtAction(nameof(GetFilm), new { id = result.Data }, result);
    }

}