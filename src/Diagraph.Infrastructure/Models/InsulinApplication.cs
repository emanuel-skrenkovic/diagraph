using Diagraph.Core.Database;

namespace Diagraph.Infrastructure.Models;

public class InsulinApplication : DbEntity
{
    public int Id { get; set; }
    
    public DateTime OccurredAtUtc { get; set; }
    
    public int Units { get; set; }
    
    public int MealId { get; set; }
}