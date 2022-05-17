using AutoMapper;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Modules.Events.Api.Events.Commands;
using Diagraph.Modules.Events.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.Events;

[Authorize]
[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
{
    private readonly IUserContext    _userContext;
    private readonly EventsDbContext _context;
    private readonly IMapper         _mapper;
    private readonly IHashTool       _hashTool;
    
    public EventsController
    (
        IUserContext             userContext, 
        EventsDbContext          context, 
        IMapper                  mapper,
        IHashTool                hashTool
    )
    {
        _userContext   = userContext;
        _context       = context;   
        _mapper        = mapper;
        _hashTool      = hashTool;
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
            .Include(nameof(Event.Tags)) // TODO: remove from here, pull event
            .Where(m => m.OccurredAtUtc >= from && m.OccurredAtUtc < to)
            .OrderBy(e => e.OccurredAtUtc)
            .ToListAsync()
    );
    
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventCommand command)
    {
        Event @event         = _mapper.Map<Event>(command);
        @event.UserId        = _userContext.UserId;
        @event.Discriminator = @event.ComputeDiscriminator(_hashTool);
        
        Event createdEvent = _context.Events.Add(@event).Entity;
        await _context.SaveChangesAsync();

        return CreatedAtRoute
        (
            "GetEvent", 
            new { id = createdEvent.Id }, 
            null
        );
    }

    [HttpGet]
    [Route("{id:int}", Name="GetEvent")]
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
        [FromBody] UpdateEventCommand request 
    )
    {
        Event @event = await _context
            .Events
            .Include(nameof(Event.Tags))
            .WithUser(_userContext.UserId)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (@event is null) return NotFound();

        _mapper.Map(request, @event);
        @event.Discriminator = @event.ComputeDiscriminator(_hashTool);

        _context.Update(@event);
        await _context.SaveChangesAsync();

        return Ok();
    }
    
    [HttpGet]
    [Route("tags")]
    public async Task<IActionResult> GetTags()
    {
        return Ok
        (
            await _context
                .Events
                .WithUser(_userContext.UserId)
                .Include(nameof(Event.Tags))
                .SelectMany(e => e.Tags)
                .DistinctByField(t => t.Name)
                .ToListAsync()
        );
    }
}