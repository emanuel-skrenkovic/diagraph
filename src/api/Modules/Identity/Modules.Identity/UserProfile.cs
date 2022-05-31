using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Dynamic;

namespace Diagraph.Modules.Identity;

public class UserProfile : DbEntity, IDynamicDataContainer
{
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string Data { get; set; }

    public static UserProfile Create(Guid userId)
    {
        if (userId == default) 
            throw new ArgumentException($"{nameof(userId)} cannot be equal to the default value.");
        
        return new() { UserId = userId };   
    }
}