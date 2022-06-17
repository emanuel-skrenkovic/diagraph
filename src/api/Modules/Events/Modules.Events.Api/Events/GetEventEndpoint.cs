using Diagraph.Modules.Events.Database;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.Events;

public class GetEventEndpoint : EndpointWithoutRequest
{
    public const string Name = "GetEvent";
    
    private readonly DbSet<Event> _events;

    public GetEventEndpoint(EventsDbContext dbContext)
        => _events = dbContext.Events;
    
    public override void Configure()
    {
        Get("events/{id}");
        Description(e => e.WithName(Name));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        int id = Route<int>("id");
        
        Event @event = await _events
            .Include(nameof(Event.Tags))
            .FirstOrDefaultAsync(e => e.Id == id, ct);
        
        await 
        (
            @event is null
                ? SendNotFoundAsync(ct) 
                : SendOkAsync(@event, ct)
        ); 
    }
}