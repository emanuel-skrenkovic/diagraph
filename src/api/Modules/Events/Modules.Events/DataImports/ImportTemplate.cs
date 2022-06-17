using Diagraph.Infrastructure;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Dynamic;

namespace Diagraph.Modules.Events.DataImports;

public class ImportTemplate : DbEntity, IUserRelated, IDynamicDataContainer
{
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string Name { get; set; }
    
    public string Data { get; set; }
}