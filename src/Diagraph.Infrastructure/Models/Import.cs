using Diagraph.Core.Database;

namespace Diagraph.Infrastructure.Models;

public class Import : DbEntity
{
    public int Id { get; set; }
    
    public string Hash { get; set; }
    
    public virtual ICollection<GlucoseMeasurement> Measurements { get; set; }
}