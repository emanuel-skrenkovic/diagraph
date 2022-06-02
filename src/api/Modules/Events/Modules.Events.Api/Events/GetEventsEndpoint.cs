using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.Events.Database;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.Events;

public class GetEventsEndpoint : EndpointWithoutRequest
{
    private readonly EventsDbContext _context;
    private readonly IUserContext    _userContext;

    public GetEventsEndpoint(EventsDbContext context, IUserContext userContext)
    {
        _context     = context;
        _userContext = userContext;
    }
    
    public override void Configure() => Get("events");

    public override async Task HandleAsync(CancellationToken ct)
    {
        var from = Query<DateTime>("from");
        var to   = Query<DateTime>("to");
        
        await SendOkAsync
        (
            await _context
                .Events
                .WithUser(_userContext.UserId)
                .Include(e => e.Tags) // TODO: remove from here, pull event
                .Where(m => m.OccurredAtUtc >= from && m.OccurredAtUtc < to)
                .OrderBy(e => e.OccurredAtUtc)
                .ToListAsync(ct),
            ct
        );
    }
}