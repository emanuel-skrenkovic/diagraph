using Diagraph.Infrastructure.Database;

namespace Diagraph.Modules.Events;

public class EventTag : DbEntity
{
    public int EventId { get; set; }
    
    public string Name { get; set; }
}