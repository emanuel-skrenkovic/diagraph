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
    private readonly EventsDbContext _context;
    private readonly GoogleFit       _fit;
    private readonly IHashTool       _hashTool;

    public ImportGoogleFitnessActivitiesEndpoint
    (
        EventsDbContext context, 
        GoogleFit       fit, 
        IHashTool       hashTool
    )
    {
        _context  = context;
        _fit      = fit;
        _hashTool = hashTool;
    }
    
    public override void Configure() => Post("events/data-import/google/fitness");

    public override async Task HandleAsync(CancellationToken ct)
    {
        // TODO: this doesn't work. Should only take GoogleFitness events into account
        Event lastFitnessEvent = await _context
            .Events
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
            .Where(s => s.Value.First().IntVal != (int) ActivityType.Still
                        && !string.IsNullOrWhiteSpace(s.StartTimeNanos) 
                        && !string.IsNullOrWhiteSpace(s.EndTimeNanos)
                        && long.Parse(s.EndTimeNanos) - long.Parse(s.StartTimeNanos) >= 15 * 6e10) // duration is more than 15 minutes
            .Select(ActivityToEvent)
            .ToList();
        
        List<DateTime> dates = fitnessEvents.Select(e => e.OccurredAtUtc).ToList();
        DateTime       start = dates.Min();
        DateTime       end   = dates.Max();
        
        List<string> discriminators = await _context
            .Events
            .Where(e => e.OccurredAtUtc >= start && e.OccurredAtUtc <= end) // TODO: think about limits
            .Select(e => e.Discriminator)
            .ToListAsync(ct);

        IEnumerable<Event> newEvents = fitnessEvents
            .ExceptBy(discriminators, e => e.Discriminator)
            .ToList();

        int rowsAffected = 0;
        if (newEvents.Any())
        {
            _context.AddRange(newEvents);
            rowsAffected = await _context.SaveChangesAsync(ct);           
        }
        
        await SendOkAsync
        (
            new ImportGoogleFitnessActivitiesResult { Count = rowsAffected }, 
            ct
        );
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
            occurredAtUtc: DateTimeHelpers
                .FromUnixTimeNanoseconds(long.Parse(dataPoint.StartTimeNanos)),
            endedAtUtc:    DateTimeHelpers
                .FromUnixTimeNanoseconds(long.Parse(dataPoint.EndTimeNanos))
        ); 
    }
}