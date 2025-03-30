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

        var dto = new ActorResponseDTO(result.Data!, false);
        
        return CreatedAtAction(nameof(GetActor), new { id = dto.Id}, dto);
    }

    [HttpPut("changename/{id}")]
    public async Task<IActionResult> ChangeActorName([FromRoute] Guid id, [FromQuery] string name)
    {
        
        var result = await _actorService.UpdateActorInfo(id, name);

        if  (!result.Success) result.ToActionResult(_logger);
        
        return CreatedAtAction(nameof(GetActor), new { actorName = name }, result.Data);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteActor([FromRoute] string name)
    {
        var result = await _actorService.RemoveActor(name);
        
        if  (!result.Success) result.ToActionResult(_logger);

        return Ok(result.Data);
    }

    [HttpPut("{id}/films")]
    public async Task<IActionResult> ModifyFilms([FromBody] ActorRequestDTO dto)
    {

        var result = await _actorService.ModifyActorsFilms(dto);
        
        if  (!result.Success) result.ToActionResult(_logger);
        
        return Ok(result.Data);
    }
    
    
    
}