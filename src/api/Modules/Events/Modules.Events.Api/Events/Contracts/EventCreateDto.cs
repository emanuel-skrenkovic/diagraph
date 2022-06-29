using Diagraph.Modules.Events.Api.Contracts;

namespace Diagraph.Modules.Events.Api.Events.Contracts;

public class EventCreateDto
{
    public string Text { get; set; }
  
    public DateTime OccurredAtUtc { get; set; }

    public DateTime? EndedAtUtc { get; set; } = null;
      
    public ICollection<EventTagDto> Tags { get; set; }   
}