using System.ComponentModel.DataAnnotations;
using Diagraph.Modules.Events.Api.Contracts;

namespace Diagraph.Modules.Events.Api.Events.Contracts;

public class UpdateEventCommand
{
    [Required] public string Text { get; set; }
    
    [Required] public DateTime OccurredAtUtc { get; set; }
    
    public ICollection<EventTagDto> Tags { get; set; }
}