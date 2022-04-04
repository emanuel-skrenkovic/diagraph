using Diagraph.Core.Database;

namespace Diagraph.Infrastructure.Models;

public class Event : DbEntity
{
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string Text { get; set; }
    
    public DateTime OccurredAtUtc { get; set; }
    
    public string CustomData { get; set; }
    
    public ICollection<EventTag> Tags { get; set; }
}