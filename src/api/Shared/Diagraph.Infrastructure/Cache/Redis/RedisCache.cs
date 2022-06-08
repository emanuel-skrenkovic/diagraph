using System.Text.Json;
using StackExchange.Redis;

namespace Diagraph.Infrastructure.Cache.Redis;

public class RedisCache : ICache
{
    private readonly RedisConfiguration    _configuration;
    private readonly ConnectionMultiplexer _multiplexer;

    public RedisCache(RedisConfiguration configuration, ConnectionMultiplexer multiplexer)
    {
        _configuration = configuration;
        _multiplexer   = multiplexer;
    }

    private IDatabase Database => _multiplexer.GetDatabase(_configuration.Database);
    
    public T Get<T, TKey>(TKey key)
    {
        Ensure.NotNull(key);
        
        RedisValue value = Database.StringGet(Key(key));
        if (value.IsNull) return default;
        
        return JsonSerializer.Deserialize<T>((string) value);
    }

    public async Task<T> GetAsync<T, TKey>(TKey key)
    {
        Ensure.NotNull(key);
        
        RedisValue value = await Database.StringGetAsync(Key(key));
        if (value.IsNull) return default;
        
        return JsonSerializer.Deserialize<T>((string) value);
    }

    public void Set<T, TKey>(TKey key, T value, TimeSpan? timeToLive = null)
    {
        Ensure.NotNull(value);
        Database.StringSet(Key(key), JsonSerializer.Serialize(value), timeToLive);
    }

    public Task SetAsync<T, TKey>(TKey key, T value, TimeSpan? timeToLive = null)
    {
        Ensure.NotNull(value);
        return Database.StringSetAsync
        (
            Key(key), 
            JsonSerializer.Serialize(value), 
            timeToLive
        );
    }
    
    public T GetOrAdd<T, TKey>(TKey key, T value, TimeSpan? timeToLive = null)
    {
        Ensure.NotNull(key);
        
        RedisKey redisKey = Key(key);

        bool   lockTaken = false;
        string lockKey   = $"{redisKey}_lock";
        string lockValue = Guid.NewGuid().ToString();
        
        IDatabase db = Database;
        try
        {
            lockTaken = db.LockTake(lockKey, lockValue, TimeSpan.FromSeconds(5)); // TODO
            if (!lockTaken)
                throw new InvalidOperationException($"Failed to acquire lock on key '{redisKey}'.");
            
            RedisValue redisValue = db.StringGet(redisKey);
            if (!redisValue.IsNull) return JsonSerializer.Deserialize<T>((string)redisValue);
            
            db.StringSet(redisKey, JsonSerializer.Serialize(value), timeToLive);
            return value;
        }
        finally
        {
            if (lockTaken) db.LockRelease(lockKey, lockValue);
        }
    }

    public async Task<T> GetOrAddAsync<T, TKey>(TKey key, T value, TimeSpan? timeToLive = null)
    {
        Ensure.NotNull(key);

        RedisKey redisKey = Key(key);

        bool   lockTaken = false;
        string lockKey   = $"{redisKey}_lock";
        string lockValue = Guid.NewGuid().ToString();       

        IDatabase db = Database;
        try
        {
            lockTaken = await db.LockTakeAsync(lockKey, lockValue, TimeSpan.FromSeconds(5)); // TODO
            if (!lockTaken)
                throw new InvalidOperationException($"Failed to acquire lock on key '{redisKey}'.");
            
            RedisValue redisValue = await db.StringGetAsync(redisKey);
            if (!redisValue.IsNull) return JsonSerializer.Deserialize<T>((string)redisValue);

            await db.StringSetAsync(redisKey, JsonSerializer.Serialize(value), timeToLive);
            return value;
        }
        finally
        {
            if (lockTaken) await db.LockReleaseAsync(lockKey, lockValue);
        }
    }

    public bool Remove<TKey>(TKey key) => Database.KeyDelete(Key(key));

    public Task<bool> RemoveAsync<TKey>(TKey key) => Database.KeyDeleteAsync(Key(key));

    private RedisKey Key<T>(T key)
        => key is string stringKey 
            ? new RedisKey(stringKey) 
            : new RedisKey(JsonSerializer.Serialize(key));
}