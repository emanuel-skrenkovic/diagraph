using Diagraph.Modules.Events.Database;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.Events;

public class GetEventsEndpoint : EndpointWithoutRequest
{
    private readonly DbSet<Event> _events;

    public GetEventsEndpoint(EventsDbContext dbContext)
        => _events = dbContext.Events;
    
    public override void Configure() => Get("events");

    public override async Task HandleAsync(CancellationToken ct)
    {
        var from = Query<DateTime>("from");
        var to   = Query<DateTime>("to");
        
        await SendOkAsync
        (
            await _events
                .Where(m => m.OccurredAtUtc >= from && m.OccurredAtUtc < to)
                .OrderBy(e => e.OccurredAtUtc)
                .ToListAsync(ct),
            ct
        );
    }
}