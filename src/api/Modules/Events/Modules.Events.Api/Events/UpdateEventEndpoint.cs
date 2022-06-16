using AutoMapper;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Modules.Events.Api.Events.Contracts;
using Diagraph.Modules.Events.Database;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.Events;

public class UpdateEventEndpoint : Endpoint<UpdateEventCommand>
{
    private readonly IUserContext    _userContext;
    private readonly EventsDbContext _context;
    private readonly IMapper         _mapper;
    private readonly IHashTool       _hashTool;
    
    public UpdateEventEndpoint
    (
        IUserContext    userContext, 
        EventsDbContext context, 
        IMapper         mapper,
        IHashTool       hashTool
    )
    {
        _userContext = userContext;
        _context     = context;   
        _mapper      = mapper;
        _hashTool    = hashTool;
    }
    
    public override void Configure() => Put("events/{id}");

    public override async Task HandleAsync(UpdateEventCommand req, CancellationToken ct)
    {
        int id = Route<int>("id");
        
        Event @event = await _context
            .Events
            .Include(nameof(Event.Tags))
            .WithUser(_userContext.UserId)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

        if (@event is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        _mapper.Map(req, @event);
        @event.Discriminator = @event.ComputeDiscriminator(_hashTool);

        _context.Update(@event);
        await _context.SaveChangesAsync(ct);

        await SendOkAsync(ct);        
    }
}