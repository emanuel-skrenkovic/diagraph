using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.Events.Database;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.Events;

public class GetEventTagsEndpoint : EndpointWithoutRequest
{
    private readonly EventsDbContext _context;
    private readonly IUserContext    _userContext;

    public GetEventTagsEndpoint(EventsDbContext context, IUserContext userContext)
    {
        _context     = context;
        _userContext = userContext;
    }
    
    public override void Configure() => Get("events/tags");

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendOkAsync
        (
            await _context
                .Events
                .WithUser(_userContext.UserId)
                .Include(nameof(Event.Tags))
                .SelectMany(e => e.Tags)
                .DistinctByField(t => t.Name)
                .ToListAsync(ct),
            ct
        );
    }
}