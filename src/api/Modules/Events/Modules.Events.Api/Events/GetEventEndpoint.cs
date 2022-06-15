using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.Events.Database;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.Events;

public class GetEventEndpoint : EndpointWithoutRequest
{
    public const string Name = "GetEvent";
    
    private readonly EventsDbContext _context;
    private readonly IUserContext    _userContext;

    public GetEventEndpoint(EventsDbContext context, IUserContext userContext)
    {
        _context     = context;
        _userContext = userContext;
    }
    
    public override void Configure()
    {
        Get("events/{id}");
        Description(e => e.WithName(Name));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        int id = Route<int>("id");
        
        Event @event = await _context
            .Events
            .WithUser(_userContext.UserId)
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