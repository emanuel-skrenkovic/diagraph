using Diagraph.Core.Database;

namespace Diagraph.Infrastructure.Models;

public class Meal : DbEntity
{
    public int Id { get; set; }
    
    public DateTime OccurredAtUtc { get; set; }
    
    public string Text { get; set; }
    
    public MealType Type { get; set; }
    
    public virtual ICollection<InsulinApplication> InsulinApplications { get; set; }
}