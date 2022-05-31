using Diagraph.Infrastructure.Notifications;

namespace Diagraph.Modules.Events.Api.Events.Commands;

public class EventView
{
    public string Text { get; set; }
    
    public DateTime OccurredAtUtc { get; set; }
    
    public ICollection<EventTag> Tags { get; set; } 
}

public class CreateEventCommand
{
    public EventView Event { get; set; }
    
    public Notification Notification { get; set; }
}