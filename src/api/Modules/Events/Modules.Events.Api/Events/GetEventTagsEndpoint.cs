using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.Events.Database;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.Events;

public class GetEventTagsEndpoint : EndpointWithoutRequest
{
    private readonly DbSet<Event> _events;

    public GetEventTagsEndpoint(EventsDbContext dbContext) 
        => _events = dbContext.Events;
    
    public override void Configure() => Get("events/tags");

    public override async Task HandleAsync(CancellationToken ct)
        => await SendOkAsync
        (
            await _events
                .Include(nameof(Event.Tags))
                .SelectMany(e => e.Tags)
                .DistinctByField(t => t.Name)
                .ToListAsync(ct),
            ct
        );
}