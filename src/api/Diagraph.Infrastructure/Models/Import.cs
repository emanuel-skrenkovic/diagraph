using Diagraph.Core.Database;

namespace Diagraph.Infrastructure.Models;

public class Import : DbEntity, IUserRelated
{
    public int Id { get; set; }
    
    public string Hash { get; set; }
    
    public virtual ICollection<GlucoseMeasurement> Measurements { get; set; }
    
    public Guid UserId { get; set; }

    // TODO: really don't like this.
    public void WithUser(Guid userId)
    {
        UserId = userId;
        Measurements = Measurements.Select(m =>
        {
            m.UserId = userId;
            return m;
        }).ToList();
    }
}