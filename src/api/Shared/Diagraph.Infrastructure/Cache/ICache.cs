namespace Diagraph.Infrastructure.Cache;

public interface ICache
{
    T Get<T, TKey>(TKey key);
    Task<T> GetAsync<T, TKey>(TKey key);

    void Set<T, TKey>(TKey key, T value, TimeSpan? timeToLive);
    
    Task SetAsync<T, TKey>(TKey key, T value, TimeSpan? timeToLive);
    
    T GetOrAdd<T, TKey>(TKey key, T value, TimeSpan? timeToLive);
    
    Task<T> GetOrAddAsync<T, TKey>(TKey key, T value, TimeSpan? timeToLive); 
}