using Diagraph.Core.Database;

namespace Diagraph.Infrastructure.Models;

public class EventTag : DbEntity
{
    public int EventId { get; set; }
    
    public string Name { get; set; }
}