using AutoMapper;
using Diagraph.Infrastructure;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
{
    private readonly IUserContext      _userContext;
    // TODO: have a single event entity with tags, add extra data as jsonb
    private readonly DiagraphDbContext _context;
    private readonly IMapper           _mapper;
    private readonly IHashTool         _hashTool;

    public EventsController
    (
        IUserContext userContext, 
        DiagraphDbContext context, 
        IMapper mapper,
        IHashTool hashTool
    )
    {
        _userContext = userContext;
        _context     = context;   
        _mapper      = mapper;
        _hashTool    = hashTool;
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
            .WithUser(_userContext.UserId)
            .Where(m => m.OccurredAtUtc >= from && m.OccurredAtUtc < to)
            .ToListAsync()
    );
    
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] Event @event)
    {
        @event.UserId        = _userContext.UserId;
        @event.Discriminator = @event.ComputeDiscriminator(_hashTool);
        
        Event createdEvent = _context.Events.Add(@event).Entity;
        await _context.SaveChangesAsync();

        return CreatedAtRoute("GetEvent", new { id = createdEvent.Id }, null);
    }

    [HttpGet]
    [Route("{id:int}", Name = "GetEvent")]
    public async Task<IActionResult> GetEvent([FromRoute] int id)
    {
        Event @event = await _context
            .Events
            .WithUser(_userContext.UserId)
            .FirstOrDefaultAsync(e => e.Id == id);
        
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
        Event @event = await _context
            .Events
            .WithUser(_userContext.UserId)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (@event is null) return NotFound();

        @event               = _mapper.Map(newEventData, @event);
        @event.Discriminator = @event.ComputeDiscriminator(_hashTool);

        _context.Update(@event);
        await _context.SaveChangesAsync();

        return Ok();
    }
}