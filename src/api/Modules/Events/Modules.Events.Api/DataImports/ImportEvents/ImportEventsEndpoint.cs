using Diagraph.Infrastructure.Api.Extensions;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports;
using Diagraph.Modules.Events.DataImports.Contracts;
using Diagraph.Modules.Events.DataImports.Csv;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Diagraph.Modules.Events.Api.DataImports.ImportEvents;

public class ImportEventsEndpoint : EndpointWithoutRequest
{
    private readonly EventsDbContext          _context;
    private readonly IUserContext             _userContext;
    private readonly IHashTool                _hashTool;
    private readonly IEventTemplateDataParser _dataParser;

    public ImportEventsEndpoint
    (
        EventsDbContext          context, 
        IUserContext             userContext,
        IHashTool                hashTool,
        IEventTemplateDataParser dataParser
    )
    {
        _context     = context;
        _userContext = userContext;
        _hashTool    = hashTool;
        _dataParser  = dataParser;
    }
    
    public override void Configure() => Post("events/data-import");

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!Form.TryGetValue("templateName", out StringValues templateNameParam))
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        string templateName = templateNameParam;

        IFormFile file = Files.GetFile("file");
        if (file is null)
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        ImportTemplate template = await _context
                .Templates
                .WithUser(_userContext.UserId)
                .FirstOrDefaultAsync(t => t.Name == templateName, ct);

        if (template is null)
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        List<Event> events = _dataParser
            .Parse
            (
                await file.ReadAsync(), 
                template.Get<CsvTemplate>()
            )
            .Select(e =>
            {
                e.UserId        = _userContext.UserId;
                e.Discriminator = e.ComputeDiscriminator(_hashTool);
                
                return e;
            }).ToList();

        if (!events.Any())
        {
            await SendOkAsync(ct);
            return;
        }

        List<DateTime> dates = events.Select(e => e.OccurredAtUtc).ToList();
        DateTime       from  = dates.Min();
        DateTime       to    = dates.Max();

        List<string> discriminators = await _context
            .Events
            .WithUser(_userContext.UserId)
            .Where(e => e.OccurredAtUtc >= from && e.OccurredAtUtc <= to) // TODO: think about limits
            .Select(e => e.Discriminator)
            .ToListAsync(ct);

        IEnumerable<Event> newEvents = events
            .Where(e => !discriminators.Contains(e.Discriminator))
            .ToList();

        if (newEvents.Any())
        {
            _context.AddRange(newEvents);
            await _context.SaveChangesAsync(ct); 
        }

        await SendOkAsync(ct);
    }
}