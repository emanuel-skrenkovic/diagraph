using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataExports;
using Diagraph.Modules.Events.DataExports.Contracts;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.DataExports;

public class ExportEventsEndpoint : EndpointWithoutRequest
{
    private readonly DbSet<Event>          _events;
    private readonly DbSet<ExportTemplate> _exportTemplates;
    private readonly ExportStrategyContext _exportContext;
    private readonly ITemplatedEventExport  _csvEventExport;

    public ExportEventsEndpoint
    (
        EventsDbContext       dbContext, 
        ExportStrategyContext exportContext,
        ITemplatedEventExport csvEventExport
    )
    {
        _events          = dbContext.Events;
        _exportTemplates = dbContext.ExportTemplates;
        _exportContext   = exportContext;
        _csvEventExport     = csvEventExport;
    }
    
    public override void Configure() => Get("events/data-export/csv");

    public override async Task HandleAsync(CancellationToken ct)
    {
        string exportTemplateName = Query<string>("template", isRequired: true);
        bool mergeSequential      = Query<bool>("mergeSequential", isRequired: false);

        IEnumerable<Event> events = await _events
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

        ExportTemplate exportTemplate = await _exportTemplates.FirstOrDefaultAsync
        (
            t => t.Name == exportTemplateName, 
            ct
        );

        if (exportTemplate is null)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        IEnumerable<Event> eventsToExport = strategy.Run(events);
        await SendBytesAsync
        (
            await _csvEventExport.ExportEventsAsync(exportTemplate, eventsToExport),
            fileName:     $"diagraph_events_{DateTime.UtcNow:yyyy-M-d}.csv",
            cancellation: ct
        );
    }
}