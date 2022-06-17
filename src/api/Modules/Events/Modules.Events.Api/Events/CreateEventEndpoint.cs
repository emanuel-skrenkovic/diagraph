using AutoMapper;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Infrastructure.Notifications;
using Diagraph.Modules.Events.Api.Events.Contracts;
using Diagraph.Modules.Events.Database;
using FastEndpoints;

namespace Diagraph.Modules.Events.Api.Events;

public class CreateEventEndpoint : Endpoint<CreateEventCommand>
{
    private readonly EventsDbContext        _dbContext;
    private readonly IMapper                _mapper;
    private readonly IHashTool              _hashTool;
    private readonly INotificationScheduler _notificationScheduler;
    
    public CreateEventEndpoint
    (
        EventsDbContext        dbContext, 
        IMapper                mapper,
        IHashTool              hashTool,
        INotificationScheduler notificationScheduler
    )
    {
        _dbContext             = dbContext;   
        _mapper                = mapper;
        _hashTool              = hashTool;
        _notificationScheduler = notificationScheduler;
    }
    
    public override void Configure() => Post("events");

    public override async Task HandleAsync(CreateEventCommand req, CancellationToken ct)
    {
        Event @event = _mapper.Map<Event>(req.Event);
        @event.Discriminator = @event.ComputeDiscriminator(_hashTool);
        
        Event createdEvent = _dbContext.Events.Add(@event).Entity;
        
        await _dbContext.SaveChangesAsync(ct);

        Notification notification = req.Notification;
        if (notification is not null) await _notificationScheduler.ScheduleAsync(notification);

        await SendCreatedAtAsync
        (
            GetEventEndpoint.Name,
            routeValues: new { id = createdEvent.Id }, 
            null,
            cancellation: ct
        );
    }
}