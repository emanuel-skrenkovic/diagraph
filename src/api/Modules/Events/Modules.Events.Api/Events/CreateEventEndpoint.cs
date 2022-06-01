using AutoMapper;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Infrastructure.Notifications;
using Diagraph.Modules.Events.Api.Events.Commands;
using Diagraph.Modules.Events.Database;
using FastEndpoints;

namespace Diagraph.Modules.Events.Api.Events;

public class CreateEventEndpoint : Endpoint<CreateEventCommand>
{
    private readonly IUserContext           _userContext;
    private readonly EventsDbContext        _context;
    private readonly IMapper                _mapper;
    private readonly IHashTool              _hashTool;
    private readonly INotificationScheduler _notificationScheduler;
    
    public CreateEventEndpoint
    (
        IUserContext           userContext, 
        EventsDbContext        context, 
        IMapper                mapper,
        IHashTool              hashTool,
        INotificationScheduler notificationScheduler
    )
    {
        _userContext           = userContext;
        _context               = context;   
        _mapper                = mapper;
        _hashTool              = hashTool;
        _notificationScheduler = notificationScheduler;
    }
    
    public override void Configure() => Post("events");

    public override async Task HandleAsync(CreateEventCommand req, CancellationToken ct)
    {
        Event @event = _mapper.Map<Event>(req.Event);
        @event.UserId        = _userContext.UserId;
        @event.Discriminator = @event.ComputeDiscriminator(_hashTool);

        Event createdEvent = _context.Events.Add(@event).Entity;
        
        await _context.SaveChangesAsync(ct);

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