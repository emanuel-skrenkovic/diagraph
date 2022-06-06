using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataExports;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.DataExports;

public class ExportEventsEndpoint : EndpointWithoutRequest
{
    private readonly EventsDbContext       _context;
    private readonly IUserContext          _userContext;
    private readonly ExportStrategyContext _exportContext;

    public ExportEventsEndpoint
    (
        EventsDbContext       context, 
        IUserContext          userContext, 
        ExportStrategyContext exportContext 
    )
    {
        _context       = context;
        _userContext   = userContext;
        _exportContext = exportContext;
    }
    
    public override void Configure() => Get("events/data-export/csv");

    public override async Task HandleAsync(CancellationToken ct)
    {
        bool mergeSequential = Query<bool>("mergeSequential", isRequired: false);

        IEnumerable<Event> events = await _context
            .Events
            .WithUser(_userContext.UserId)
            .Include(nameof(Event.Tags))
            .ToListAsync(ct);

        if (!events.Any())
        {
            await SendOkAsync(ct);
            return;
        }

        IDataExportStrategy strategy = _exportContext.GetStrategy
        (
            mergeSequential
                ? DataExportStrategy.Merging
                : DataExportStrategy.Individual
        );

        await SendBytesAsync
        (
            await strategy.ExportAsync(events), 
            fileName:     $"diagraph_events_{DateTime.UtcNow:yyyy-M-d}.csv",
            cancellation: ct
        );
    }
}