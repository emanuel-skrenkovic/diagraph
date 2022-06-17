using Diagraph.Modules.Events.Database;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.Events;

public class DeleteEventEndpoint : EndpointWithoutRequest
{
    private readonly EventsDbContext _dbContext;

    public DeleteEventEndpoint(EventsDbContext dbContext)
        => _dbContext = dbContext;

    public override void Configure() => Delete("events/{id}");

    public override async Task HandleAsync(CancellationToken ct)
    {
        int id = Route<int>("id");
        
        Event @event = await _dbContext
            .Events
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken: ct);
        if (@event is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        _dbContext.Remove(@event);
        await _dbContext.SaveChangesAsync(ct);
        
        await SendNoContentAsync(ct);
    }
}