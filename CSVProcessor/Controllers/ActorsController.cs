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
    public async Task<IActionResult> GetActors([FromQuery] bool includeFilms = false)
    {
        
        var actors = await _actorService.GetAllActors(includeFilms);

        if  (!actors.Success) actors.ToActionResult(_logger);

        return Ok(actors.Data);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetActor( string name, [FromQuery] bool includeFilms = false )
    {
        var actor = await _actorService.GetActor(name, includeFilms: includeFilms);

        if  (!actor.Success) actor.ToActionResult(_logger);

        return Ok(actor.Data);
    }

    [HttpPost]
    public async Task<IActionResult> CreateActor([FromBody] ActorRequestDTO actorRequestData)
    {
        var result = await _actorService.CreateActor(actorRequestData);

        if  (!result.Success) result.ToActionResult(_logger);
        
        return CreatedAtAction(nameof(GetActor), new { actorName = actorRequestData.Name }, result.Data);
    }

    [HttpPut("changename/{id}")]
    public async Task<IActionResult> ChangeActorName([FromRoute] Guid id, [FromQuery] string name)
    {
        
        var result = await _actorService.UpdateActorInfo(id, name);

        if  (!result.Success) result.ToActionResult(_logger);
        
        return CreatedAtAction(nameof(GetActor), new { actorName = name }, result.Data);
    }

    [HttpDelete("deleteactor/{name}")]
    public async Task<IActionResult> DeleteActor([FromRoute] string name)
    {
        var result = await _actorService.RemoveActor(name);
        
        if  (!result.Success) result.ToActionResult(_logger);

        return Ok(result.Data);
    }

    [HttpPut("addfilms/")]
    public async Task<IActionResult> AddFilms([FromBody] ActorRequestDTO dto)
    {

        var result = await _actorService.AddActorFilms(dto);
        
        if  (!result.Success) result.ToActionResult(_logger);
        
        return Ok(result.Data);
    }

    [HttpPut("removefilms/")]
    public async Task<IActionResult> RemoveFilms([FromBody] ActorRequestDTO dto)
    {
        var result = await _actorService.RemoveActorsFilms(dto);
        
        if  (!result.Success) result.ToActionResult(_logger);
        
        return Ok(result.Data);
    }
    
    
}