using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataExports;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.DataExports;

public class ExportEventsEndpoint : EndpointWithoutRequest
{
    private readonly EventsDbContext       _dbContext;
    private readonly ExportStrategyContext _exportContext;

    public ExportEventsEndpoint
    (
        EventsDbContext       dbContext, 
        ExportStrategyContext exportContext 
    )
    {
        _dbContext     = dbContext;
        _exportContext = exportContext;
    }
    
    public override void Configure() => Get("events/data-export/csv");

    public override async Task HandleAsync(CancellationToken ct)
    {
        bool mergeSequential = Query<bool>("mergeSequential", isRequired: false);

        IEnumerable<Event> events = await _dbContext
            .Events
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