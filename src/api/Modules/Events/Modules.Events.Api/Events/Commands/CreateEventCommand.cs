namespace Diagraph.Modules.Events.Api.Events.Commands;

public class CreateEventCommand
{
    public string Text { get; set; }
    
    public DateTime OccurredAtUtc { get; set; }
    
    public ICollection<EventTag> Tags { get; set; }
}