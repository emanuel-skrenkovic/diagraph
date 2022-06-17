using AutoMapper;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Modules.Events.Api.Events.Contracts;
using Diagraph.Modules.Events.Database;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.Events;

public class UpdateEventEndpoint : Endpoint<UpdateEventCommand>
{
    private readonly EventsDbContext _dbContext;
    private readonly IMapper         _mapper;
    private readonly IHashTool       _hashTool;
    
    public UpdateEventEndpoint
    (
        EventsDbContext dbContext, 
        IMapper         mapper,
        IHashTool       hashTool
    )
    {
        _dbContext = dbContext;   
        _mapper    = mapper;
        _hashTool  = hashTool;
    }
    
    public override void Configure() => Put("events/{id}");

    public override async Task HandleAsync(UpdateEventCommand req, CancellationToken ct)
    {
        int id = Route<int>("id");
        
        Event @event = await _dbContext
            .Events
            .Include(nameof(Event.Tags))
            .FirstOrDefaultAsync(e => e.Id == id, ct);

        if (@event is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        _mapper.Map(req, @event);
        @event.Discriminator = @event.ComputeDiscriminator(_hashTool);

        _dbContext.Update(@event);
        await _dbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);        
    }
}