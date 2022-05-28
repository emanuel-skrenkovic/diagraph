using Diagraph.Infrastructure;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Dynamic;

namespace Diagraph.Modules.Identity;

public class UserProfile : DbEntity, IDynamicDataContainer
{
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string Data { get; set; }
}