using System.Text.Json;

namespace Diagraph.Infrastructure.Dynamic.Extensions;

public static class DynamicDataContainerExtensions
{
    public static void UpdateData<T, TData>(this T container, Action<TData> update)
        where T : IDynamicDataContainer
    {
        TData data = container.GetData<T, TData>();
        update(data);
        container.SetData(data);
    }
    
    public static void AddOrUpdateData<T, TData>(this T container, Action<TData> update)
        where T : IDynamicDataContainer
        where TData : new()
    {
        TData data = container.GetData<T, TData>() ?? new();
        update(data);
        container.SetData(data);
    }
    
    public static void UpdateData<T, TData>(this T container, Func<TData, TData> update)
        where T : IDynamicDataContainer
    {
        TData data = container.GetData<T, TData>();
        container.SetData(update(data));
    }

    public static void AddOrUpdateData<T, TData>(this T container, Func<TData, TData> update)
        where T : IDynamicDataContainer
        where TData : new()
    {
        TData data = container.GetData<T, TData>() ?? new();
        container.SetData(update(data));
    }
    
    public static TData GetData<T, TData>(this T container) 
        where T : IDynamicDataContainer
        => container?.Data is null 
            ? default 
            : JsonSerializer.Deserialize<TData>(container.Data);
    
    public static TData GetData<TData>(this IDynamicDataContainer container) 
        => container?.Data is null 
            ? default 
            : JsonSerializer.Deserialize<TData>(container.Data);
    
    public static void SetData<T, TData>(this T container, TData data) 
        where T : IDynamicDataContainer
        => container.Data = JsonSerializer.Serialize(data);
}