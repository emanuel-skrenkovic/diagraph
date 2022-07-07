using Diagraph.Infrastructure.Hashing;
using Diagraph.Infrastructure.Integrations.Google.Fit;
using Diagraph.Infrastructure.Integrations.Google.Fit.Contracts;
using Diagraph.Infrastructure.Time;
using Diagraph.Modules.Events.Api.DataImports.ImportEvents.Contracts;
using Diagraph.Modules.Events.Database;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.DataImports.ImportEvents;

public class ImportGoogleFitnessActivitiesEndpoint : EndpointWithoutRequest
{
    private const string GoogleFitnessSource = "google_fit";
    
    private readonly EventsDbContext _context;
    private readonly GoogleFit       _fit;
    private readonly IHashTool       _hashTool;
    private readonly EventImport     _eventImport;

    public ImportGoogleFitnessActivitiesEndpoint
    (
        EventsDbContext context, 
        GoogleFit       fit, 
        IHashTool       hashTool,
        EventImport     eventImport
    )
    {
        _context     = context;
        _fit         = fit;
        _hashTool    = hashTool;
        _eventImport = eventImport;
    }
    
    public override void Configure() => Post("events/data-import/google/fitness");

    public override async Task HandleAsync(CancellationToken ct)
    {
        Event lastFitnessEvent = await _context
            .Events
            .Where(e => e.Source == GoogleFitnessSource)
            .OrderByDescending(e => e.OccurredAtUtc)
            .Take(1)
            .SingleOrDefaultAsync(cancellationToken: ct);
        
        List<DataPoint> fitnessSessions = 
        (
            await _fit.GetActivitiesAsync(lastFitnessEvent?.OccurredAtUtc, DateTime.UtcNow.AddDays(1))
        )?.ToList();

        if (fitnessSessions?.Any() != true)
        {
            await SendOkAsync(ct);
            return;
        }
        
        List<Event> fitnessEvents = fitnessSessions
            .Where(s => (s.Value.First().IntVal != (int) ActivityType.Still 
                         || s.Value.First().IntVal != (int) ActivityType.InVehicle)
                        && !string.IsNullOrWhiteSpace(s.StartTimeNanos) 
                        && !string.IsNullOrWhiteSpace(s.EndTimeNanos)
                        // duration is more than 15 minutes
                        && long.Parse(s.EndTimeNanos) - long.Parse(s.StartTimeNanos) >= 15 * 6e10) 
            .Select(ActivityToEvent)
            .ToList();

        int eventsCreated = await _eventImport.ExecuteAsync(fitnessEvents, ct);
        await SendOkAsync(new ImportGoogleFitnessActivitiesResult { Count = eventsCreated }, ct);
    }

    private Event ActivityToEvent(DataPoint dataPoint)
    {
        string activity = dataPoint.Value.First().IntVal.ToString();
                    
        if (!Enum.TryParse(activity, out ActivityType activityType))
            activityType = ActivityType.Unknown;

        return Event.Create
        (
            hashTool:      _hashTool,
            text:          activityType.ToString(),
            source:        GoogleFitnessSource,
            occurredAtUtc: DateTimeHelpers
                .FromUnixTimeNanoseconds(long.Parse(dataPoint.StartTimeNanos)),
            endedAtUtc:    DateTimeHelpers
                .FromUnixTimeNanoseconds(long.Parse(dataPoint.EndTimeNanos))
        ); 
    }
}