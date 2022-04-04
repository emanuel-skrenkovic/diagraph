using Diagraph.Core.Database;

namespace Diagraph.Infrastructure.Models;

public class UserProfile : DbEntity
{
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string Data { get; set; }
}