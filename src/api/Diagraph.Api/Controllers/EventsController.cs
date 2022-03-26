using AutoMapper;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
{
    // TODO: have a single event entity with tags, add extra data as jsonb
    private readonly DiagraphDbContext _context;
    private readonly IMapper _mapper;

    public EventsController(DiagraphDbContext context, IMapper mapper)
    {
        _context = context;   
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents
    (
        [FromQuery] DateTime from,
        [FromQuery] DateTime to
    ) => Ok
    (
        await _context
            .Events
            .Where(m => m.OccurredAtUtc >= from && m.OccurredAtUtc < to)
            .ToListAsync()
    );
    
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] Event @event)
    {
        Event createdEvent = _context.Events.Add(@event).Entity;
        await _context.SaveChangesAsync();

        return CreatedAtRoute("GetEvent", new { id = createdEvent.Id }, null);
    }

    [HttpGet]
    [Route("{id:int}", Name = "GetEvent")]
    public async Task<IActionResult> GetEvent([FromRoute] int id)
    {
        Event @event = await _context.FindAsync<Event>(id);
        return @event is null ? NotFound() : Ok(@event);
    }

    [HttpPut]
    [Route("{id:int}")]
    public async Task<IActionResult> UpdateEvent
    (
        [FromRoute] int id, 
        [FromBody] Event newEventData
    )
    {
        Event @event = await _context.FindAsync<Event>(id);
        if (@event is null) return NotFound();

        _mapper.Map(newEventData, @event);

        _context.Update(@event);
        await _context.SaveChangesAsync();

        return Ok();
    }
}