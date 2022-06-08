namespace Diagraph.Infrastructure.Cache;

public interface ICache
{
    T Get<T, TKey>(TKey key);
    Task<T> GetAsync<T, TKey>(TKey key);

    void Set<T, TKey>(TKey key, T value, TimeSpan? timeToLive = null);
    
    Task SetAsync<T, TKey>(TKey key, T value, TimeSpan? timeToLive = null);
    
    T GetOrAdd<T, TKey>(TKey key, T value, TimeSpan? timeToLive = null);
    
    Task<T> GetOrAddAsync<T, TKey>(TKey key, T value, TimeSpan? timeToLive = null); 
    
    bool Remove<TKey>(TKey key);
    
    Task<bool> RemoveAsync<TKey>(TKey key);
}