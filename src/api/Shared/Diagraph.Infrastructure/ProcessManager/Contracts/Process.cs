using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Dynamic;

namespace Diagraph.Infrastructure.ProcessManager.Contracts;

public class Process : DbEntity, IDynamicDataContainer
{
    public int Id { get; set; }
    
    public string ProcessId { get; set; }
    
    public bool Initiated { get; set; }
    
    public bool Finished { get; set; }
   
    public string Data { get; set; }
}