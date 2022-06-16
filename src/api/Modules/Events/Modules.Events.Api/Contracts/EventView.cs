namespace Diagraph.Modules.Events.Api.Contracts;

public class EventView
{
     public int Id { get; set; }
     
     public string Text { get; set; }
 
     public DateTime OccurredAtUtc { get; set; }
     
     public ICollection<EventTagDto> Tags { get; set; }   
}