using System.Text.Json;

namespace Diagraph.Infrastructure.Dynamic;

public interface IDynamicDataContainer
{
    public string Data { get; set; }

    public TData GetData<TData>() 
        => Data is null 
            ? default 
            : JsonSerializer.Deserialize<TData>(Data);
}