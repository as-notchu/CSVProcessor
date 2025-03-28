using CSVProcessor.Database;
using CSVProcessor.Helpers;
using CSVProcessor.Interfaces;
using CSVProcessor.Models.DTO;
using CSVProcessor.Services;
using Microsoft.AspNetCore.Mvc;

namespace CSVProcessor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActorsController : ControllerBase
{
    private readonly CsvContext _context;
    
    private readonly ILogger<ActorsController> _logger;

    private readonly ActorService _actorService;


    public ActorsController(CsvContext context, ILogger<ActorsController> logger, ActorService actorService)
    {
        _context = context;
        _logger = logger;
        _actorService = actorService;
    }




    [HttpGet]
    public async Task<IActionResult> GetActors([FromRoute] bool includeFilms = false)
    {
        
        var actors = await _actorService.GetAllActors(includeFilms);

        if (!actors.Success)
        {
            return actors.ToActionResult(_logger);
        }

        return Ok(actors);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetActor([FromBody] string name, bool includeFilms = false )
    {
        var actor = await _actorService.GetActor(name, includeFilms: includeFilms);

        if (!actor.Success)
        {
            return actor.ToActionResult(_logger);
        }

        return Ok(new ActorResponseDTO(actor.Data!, includeFilms));
    }

    [HttpPost]
    public async Task<IActionResult> CreateActor([FromBody] ActorRequestDTO actorRequestData)
    {
        var result = await _actorService.CreateActor(actorRequestData);

        if (!result.Success)
        {
            return result.ToActionResult(_logger);
        }
        
        return CreatedAtAction(nameof(GetActor), new { actorName = actorRequestData.Name }, result.Data);
    }
}