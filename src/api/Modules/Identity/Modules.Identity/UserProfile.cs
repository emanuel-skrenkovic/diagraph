using Diagraph.Infrastructure.Database;

namespace Diagraph.Modules.Identity;

public class UserProfile : DbEntity
{
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string Data { get; set; }
}