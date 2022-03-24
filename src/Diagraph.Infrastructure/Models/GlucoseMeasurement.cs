using Diagraph.Core.Database;

namespace Diagraph.Infrastructure.Models;

public class GlucoseMeasurement : DbEntity
{
    public long Id { get; set; }
    
    public int Level { get; set; }
    
    public GlucoseUnit Unit { get; set; } 
    
    public int ImportId { get; set; }
}