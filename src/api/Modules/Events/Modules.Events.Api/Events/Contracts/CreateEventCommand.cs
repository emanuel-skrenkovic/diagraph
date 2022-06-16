using Diagraph.Infrastructure.Notifications;
using Diagraph.Modules.Events.Api.Contracts;

namespace Diagraph.Modules.Events.Api.Events.Contracts;

public class CreateEventCommand
{
    public EventCreateDto Event { get; set; }
    
    public Notification Notification { get; set; }
}