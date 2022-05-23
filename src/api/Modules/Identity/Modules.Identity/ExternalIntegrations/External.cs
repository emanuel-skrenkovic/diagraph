using System.Text.Json;
using Diagraph.Infrastructure;

namespace Diagraph.Modules.Identity.ExternalIntegrations;

public enum ExternalProvider
{
    Google
}

public class External : IUserRelated
{
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public ExternalProvider Provider { get; set; }
    
    public string Data { get; set; }

    public T GetData<T>() => JsonSerializer.Deserialize<T>(Data);

    public void SetData<T>(T data) => Data = JsonSerializer.Serialize(data);
}