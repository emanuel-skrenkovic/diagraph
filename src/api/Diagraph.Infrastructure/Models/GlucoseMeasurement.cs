using Diagraph.Core.Database;

namespace Diagraph.Infrastructure.Models;

public class GlucoseMeasurement : DbEntity, IUserRelated
{
    public long Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public float Level { get; set; }
    
    public DateTime TakenAt { get; set; }

    public GlucoseUnit Unit { get; set; } = GlucoseUnit.MmolPerLitre;
    
    public int ImportId { get; set; }
}