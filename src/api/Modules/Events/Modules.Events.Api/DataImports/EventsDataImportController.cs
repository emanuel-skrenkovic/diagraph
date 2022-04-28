using Diagraph.Infrastructure.Api.Extensions;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Modules.Events.Api.DataImports.Commands;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports;
using Diagraph.Modules.Events.DataImports.Contracts;
using Diagraph.Modules.Events.DataImports.Csv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.DataImports;

[Authorize]
[ApiController]
[Route("events/data-import")]
public class EventsDataImportController : ControllerBase
{
    private readonly IUserContext             _userContext;
    private readonly EventsDbContext          _context;
    private readonly IHashTool                _hashTool;
    private readonly IEventTemplateDataParser _dataParser;

    public EventsDataImportController
    (
        IUserContext             userContext,
        EventsDbContext          context,
        IHashTool                hashTool,
        IEventTemplateDataParser dataParser
    )
    {
        _userContext = userContext;
        _context     = context;
        _hashTool    = hashTool;
        _dataParser  = dataParser;
    }
    
    [HttpPost]
    public async Task<IActionResult> ImportEvents(ImportEventsCommand command)
    {
        if (command.File is null) return BadRequest();

        ImportTemplate template = await _context
            .Templates
            .WithUser(_userContext.UserId)
            .FirstOrDefaultAsync(t => t.Name == command.TemplateName);

        if (template is null) return BadRequest();

        List<Event> events = _dataParser
            .Parse
            (
                await command.File.ReadAsync(), 
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
    [Route("dry-run")]
    public async Task<IActionResult> ImportEventsDryRun(IFormFile file, [FromQuery] string templateName) // TODO: consolidate with actual request.
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