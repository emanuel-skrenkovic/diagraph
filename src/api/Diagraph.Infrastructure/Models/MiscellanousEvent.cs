using Diagraph.Core.Database;

namespace Diagraph.Infrastructure.Models;

public class MiscellanousEvent : DbEntity
{
    public int Id { get; set; }
    
    public DateTime OccurredAtUtc { get; set; }
    
    public string Note { get; set; }
}