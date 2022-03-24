namespace Diagraph.Core.Database;

public abstract class DbEntity
{
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime UpdatedAtUtc { get; set; }
}