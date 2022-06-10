using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.Events.Database;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.Events;

public class DeleteEventEndpoint : EndpointWithoutRequest
{
    private readonly EventsDbContext _context;
    private readonly IUserContext    _userContext;

    public DeleteEventEndpoint(EventsDbContext context, IUserContext userContext)
    {
        _context     = context;
        _userContext = userContext;
    }

    public override void Configure() => Delete("events/{id}");

    public override async Task HandleAsync(CancellationToken ct)
    {
        int id = Route<int>("id");
        
        Event @event = await _context
            .Events
            .WithUser(_userContext.UserId)
            .SingleOrDefaultAsync(e => e.Id == id, ct);

        if (@event is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        _context.Remove(@event);
        await _context.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}