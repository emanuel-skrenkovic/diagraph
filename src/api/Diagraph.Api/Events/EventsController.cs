using AutoMapper;
using Diagraph.Api.Events.Models;
using Diagraph.Core.Extensions;
using Diagraph.Infrastructure;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Infrastructure.Models;
using Diagraph.Infrastructure.Parsing;
using Diagraph.Infrastructure.Parsing.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Api.Events;

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

    private readonly IEventTemplateDataParser _dataParser;

    public EventsController
    (
        IUserContext             userContext, 
        DiagraphDbContext        context, 
        IMapper                  mapper,
        IHashTool                hashTool,
        IEventTemplateDataParser dataParser
    )
    {
        _userContext   = userContext;
        _context       = context;   
        _mapper        = mapper;
        _hashTool      = hashTool;
        _dataParser    = dataParser;
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

    [HttpPost]
    [Route("imports")]
    public async Task<IActionResult> ImportEvents(ImportEventsRequest request)
    {
        if (request.File is null) return BadRequest();

        ImportTemplate template = await _context
            .Templates
            .WithUser(_userContext.UserId)
            .FirstOrDefaultAsync(t => t.Name == request.TemplateName);

        if (template is null) return BadRequest();

        List<Event> events = _dataParser
            .Parse
            (
                await request.File.ReadAsync(), 
                template.Get<CsvTemplate>()
            )
            .Select(e =>
            {
                e.UserId        = _userContext.UserId;
                e.Discriminator = e.ComputeDiscriminator(_hashTool);
                
                return e;
            }).ToList();

        if (!events.Any()) return Ok();

        List<DateTime> dates = events.Select(e => e.OccurredAtUtc).ToList();
        DateTime from        = dates.Min();
        DateTime to          = dates.Max();

        List<string> discriminators = await _context
            .Events
            .WithUser(_userContext.UserId)
            .Where(e => e.OccurredAtUtc >= from && e.OccurredAtUtc <= to) // TODO: think about limits
            .Select(e => e.Discriminator)
            .ToListAsync();

        IEnumerable<Event> newEvents = events
            .Where(e => !discriminators.Contains(e.Discriminator))
            .ToList();

        if (newEvents.Any())
        {
            _context.AddRange(newEvents);
            await _context.SaveChangesAsync(); 
        }

        return Ok();
    }

    [HttpPost]
    [Route("imports/dry-run")]
    public async Task<IActionResult> ImportEventsDryRun(IFormFile file, [FromQuery] string templateName)
    {
        if (file is null) return BadRequest();

        ImportTemplate template = await _context
            .Templates
            .WithUser(_userContext.UserId)
            .FirstOrDefaultAsync(t => t.Name == templateName);

        if (template is null) return BadRequest();
        
        return Ok
        (
            _dataParser.Parse
            (
                await file.ReadAsync(), 
                template.Get<CsvTemplate>()
            )
        );
    }
}