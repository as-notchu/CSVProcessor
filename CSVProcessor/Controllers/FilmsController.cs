
using CSVProcessor.Helpers;
using CSVProcessor.Models;
using CSVProcessor.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CSVProcessor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilmsController : ControllerBase
{
    private readonly ILogger<FilmsController> _logger;
    
    private readonly DataService _dataService;
    

    public FilmsController(ILogger<FilmsController> logger, DataService dataService)
    {
        _logger = logger;
        _dataService = dataService;
    }
    
    [HttpGet]
    [SwaggerOperation(
        Summary = "Returns all films",
        Description = "Returns all films as IEnumerable<FilmData>"
        )]
    [ProducesResponseType(typeof(IEnumerable<FilmData>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilms()
    {
        var films = await _dataService.GetFilms();
            
        return Ok(films);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get Film using Guid",
        Description = "Returns film with requested guid. 404 error if film was not found"
    )]
    [ProducesResponseType(typeof(FilmData), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetFilm(Guid id)
    {
        var film = await _dataService.GetFilmById(id);

        if (film == null)
        {
            return NotFound($"Film with ID {id} not found");
        }
            
        return Ok(film);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete Film using Guid",
        Description = "Deletes film with requested guid")]
    [ProducesResponseType(typeof(Unit), 200)]
    [ProducesResponseType( 404)]
    public async Task<IActionResult> DeleteFilm(Guid id)
    {

        var result = await _dataService.DeleteFilm(id);

        return result.ToActionResult(_logger);

    }

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update Film Data using Guid",
        Description = "Updates film with requested guid. Returns updated film data"
        )]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [ProducesResponseType(typeof(FilmData),201)]
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

        return CreatedAtAction(nameof(GetFilm), new { id = result.Data!.Id }, result.Data);

    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create Film Data from DTO",
        Description = "Creates film data with DTO. Returns created film data. "
        )]
    [ProducesResponseType(typeof(FilmData), 201)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> AddFilm([FromBody] FilmDTO filmDto)
    {
        var result = await _dataService.AddFilm(filmDto);

        if (!result.Success)
        {
            return result.ToActionResult(_logger);
        }

        return CreatedAtAction(nameof(GetFilm), new { id = result.Data }, result.Data);
    }

}