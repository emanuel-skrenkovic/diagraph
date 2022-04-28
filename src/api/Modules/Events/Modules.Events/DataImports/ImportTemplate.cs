using System.Text.Json;
using Diagraph.Infrastructure;
using Diagraph.Infrastructure.Database;

namespace Diagraph.Modules.Events.DataImports;

public class ImportTemplate : DbEntity, IUserRelated
{
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string Name { get; set; }
    
    public string Data { get; set; }

    public T Get<T>() => JsonSerializer.Deserialize<T>
    (
        Data, 
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
    );

    public void Set<T>(T template) => Data = JsonSerializer.Serialize(template);
}