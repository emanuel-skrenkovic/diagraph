using Diagraph.Infrastructure.Tests.AutoFixture;
using FluentAssertions;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Diagraph.Shared.Tests.Integration.Redis;

public class RedisCacheTests : IClassFixture<RedisFixture>
{
    private readonly RedisFixture _fixture;

    public RedisCacheTests(RedisFixture fixture) => _fixture = fixture;

    [Theory, CustomizedAutoData]
    public void Sets_String(string key, string value)
    {
        var cache = _fixture.Cache;
        
        // Act
        cache.Set(key, value);
        
        // Assert
        string cachedValue = cache.Get<string, string>(key);
        cachedValue.Should().Be(value);
    }
    
    [Theory, CustomizedAutoData]
    internal void Sets_Object(string key, Cached value)
    {
        var cache = _fixture.Cache;
        
        // Act
        cache.Set(key, value);
        
        // Assert
        Cached cached = cache.Get<Cached, string>(key);
        cached.Should().NotBeNull();

        cached.Int.Should().Be(value.Int);
        cached.String.Should().Be(value.String);
        cached.Values.Should().NotBeNull();
        cached.Values.Int.Should().Be(value.Values.Int);
        cached.Values.String.Should().Be(value.Values.String);
    }
    
    [Theory, CustomizedAutoData]
    public async Task Sets_String_Asynchronously(string key, string value)
    {
        var cache = _fixture.Cache;
        
        // Act
        await cache.SetAsync(key, value);
        
        // Assert
        string cachedValue = await cache.GetAsync<string, string>(key);
        cachedValue.Should().Be(value);
    }
    
    [Theory, CustomizedAutoData]
    internal async Task Sets_Object_Asynchronously(string key, Cached value)
    {
        var cache = _fixture.Cache;
        
        // Act
        await cache.SetAsync(key, value);
        
        // Assert
        Cached cached = await cache.GetAsync<Cached, string>(key);
        
        cached.Should().NotBeNull();
        cached.Int.Should().Be(value.Int);
        cached.String.Should().Be(value.String);
        cached.Values.Should().NotBeNull();
        cached.Values.Int.Should().Be(value.Values.Int);
        cached.Values.String.Should().Be(value.Values.String);
    }
    
    [Theory, CustomizedAutoData]
    internal void Get_Or_Add_Adds_Value(string key, string value)
    {
        // Act
        string cachedValue = _fixture.Cache.GetOrAdd(key, value);
        
        // Assert
        //string cachedValue = _fixture.Cache.Get<string, string>(key);
        cachedValue.Should().Be(value);    
    }
    
    [Theory, CustomizedAutoData]
    internal void Get_Or_Add_Adds_Value_2(string key, string value)
    {
        var cache = _fixture.Cache;
        
        // Act
        cache.GetOrAdd(key, value);
        
        // Assert
        string cachedValue = cache.Get<string, string>(key);
        cachedValue.Should().Be(value);    
    }
    
    [Theory, CustomizedAutoData]
    internal void Get_Or_Add_Adds_Object_Value(string key, Cached value)
    {
        // Act
        Cached cached = _fixture.Cache.GetOrAdd(key, value);
        
        // Assert
        cached.Should().NotBeNull();
        cached.Int.Should().Be(value.Int);
        cached.String.Should().Be(value.String);
        cached.Values.Should().NotBeNull();
        cached.Values.Int.Should().Be(value.Values.Int);
        cached.Values.String.Should().Be(value.Values.String);
    }
    
    [Theory, CustomizedAutoData]
    internal void Get_Or_Add_Adds_Object_Value_2(string key, Cached value)
    {
        var cache = _fixture.Cache;
        
        // Act
        cache.GetOrAdd(key, value);
        
        // Assert
        Cached cached = cache.Get<Cached, string>(key);
        
        cached.Should().NotBeNull();
        cached.Int.Should().Be(value.Int);
        cached.String.Should().Be(value.String);
        cached.Values.Should().NotBeNull();
        cached.Values.Int.Should().Be(value.Values.Int);
        cached.Values.String.Should().Be(value.Values.String);
    }

    [Theory, CustomizedAutoData]
    internal void Get_Or_Add_Gets_Existing_Value(string key, string oldValue, string newValue)
    {
        var cache = _fixture.Cache;
        
        // Act
        cache.Set(key, oldValue);
        
        // Assert
        string cached = cache.GetOrAdd(key, newValue);
        cached.Should().Be(oldValue);
    }
    
    [Theory, CustomizedAutoData]
    internal void Get_Or_Add_Gets_Existing_Object_Value(string key, Cached oldValue, Cached newValue)
    {
        var cache = _fixture.Cache;
        
        // Act
        cache.Set(key, oldValue);
        
        // Assert
        Cached cached = cache.GetOrAdd(key, newValue);
        
        cached.Should().NotBeNull();
        cached.Int.Should().Be(oldValue.Int);
        cached.String.Should().Be(oldValue.String);
        cached.Values.Should().NotBeNull();
        cached.Values.Int.Should().Be(oldValue.Values.Int);
        cached.Values.String.Should().Be(oldValue.Values.String);
    }
    
   [Theory, CustomizedAutoData]
    internal async Task Get_Or_Add_Adds_Value_Asynchronously(string key, string value)
    {
        // Act
        string cachedValue = await _fixture.Cache.GetOrAddAsync(key, value);
        
        // Assert
        //string cachedValue = _fixture.Cache.Get<string, string>(key);
        cachedValue.Should().Be(value);    
    }
    
    [Theory, CustomizedAutoData]
    internal async Task Get_Or_Add_Adds_Value_2_Asynchronously(string key, string value)
    {
        var cache = _fixture.Cache;
        
        // Act
        await cache.GetOrAddAsync(key, value);
        
        // Assert
        string cachedValue = await cache.GetAsync<string, string>(key);
        cachedValue.Should().Be(value);    
    }
    
    [Theory, CustomizedAutoData]
    internal async Task Get_Or_Add_Adds_Object_Value_Asynchronously(string key, Cached value)
    {
        // Act
        Cached cached = await _fixture.Cache.GetOrAddAsync(key, value);
        
        // Assert
        cached.Should().NotBeNull();
        cached.Int.Should().Be(value.Int);
        cached.String.Should().Be(value.String);
        cached.Values.Should().NotBeNull();
        cached.Values.Int.Should().Be(value.Values.Int);
        cached.Values.String.Should().Be(value.Values.String);
    }
    
    [Theory, CustomizedAutoData]
    internal async Task Get_Or_Add_Adds_Object_Value_2_Asynchronously(string key, Cached value)
    {
        var cache = _fixture.Cache;
        
        // Act
        await cache.GetOrAddAsync(key, value);
        
        // Assert
        Cached cached = await cache.GetAsync<Cached, string>(key);
        
        cached.Should().NotBeNull();
        cached.Int.Should().Be(value.Int);
        cached.String.Should().Be(value.String);
        cached.Values.Should().NotBeNull();
        cached.Values.Int.Should().Be(value.Values.Int);
        cached.Values.String.Should().Be(value.Values.String);
    }

    [Theory, CustomizedAutoData]
    internal async Task Get_Or_Add_Gets_Existing_Value_Asynchronously(string key, string oldValue, string newValue)
    {
        var cache = _fixture.Cache;
        
        // Act
        await cache.SetAsync(key, oldValue);
        
        // Assert
        string cached = await cache.GetOrAddAsync(key, newValue);
        cached.Should().Be(oldValue);
    }
    
    [Theory, CustomizedAutoData]
    internal async Task Get_Or_Add_Gets_Existing_Object_Value_Asynchronously(string key, Cached oldValue, Cached newValue)
    {
        var cache = _fixture.Cache;
        
        // Act
        await cache.SetAsync(key, oldValue);
        
        // Assert
        Cached cached = await cache.GetOrAddAsync(key, newValue);
        
        cached.Should().NotBeNull();
        cached.Int.Should().Be(oldValue.Int);
        cached.String.Should().Be(oldValue.String);
        cached.Values.Should().NotBeNull();
        cached.Values.Int.Should().Be(oldValue.Values.Int);
        cached.Values.String.Should().Be(oldValue.Values.String);
    }

    [Theory, CustomizedAutoData]
    internal void Removes_Key(string key, string value)
    {
        var cache = _fixture.Cache;

        // Arrange
        cache.Set(key, value);
        
        // Act
        bool removed = cache.Remove(key);
        
        // Assert
        removed.Should().BeTrue();
        string cached = cache.Get<string, string>(key);
        cached.Should().BeNull();
    }
    
    [Theory, CustomizedAutoData]
    internal async Task Removes_Key_Asynchronously(string key, string value)
    {
        var cache = _fixture.Cache;

        // Arrange
        await cache.SetAsync(key, value);
        
        // Act
        bool removed = await cache.RemoveAsync(key);
        
        // Assert
        removed.Should().BeTrue();
        string cached = await cache.GetAsync<string, string>(key);
        cached.Should().BeNull();
    }
    
    [Theory, CustomizedAutoData]
    internal void Remove_Returns_False_If_No_Key_Found(string key)
    {
        // Act
        bool removed = _fixture.Cache.Remove(key);
        
        // Assert
        removed.Should().BeFalse();
    }
    
    [Theory, CustomizedAutoData]
    internal async Task Remove_Returns_False_If_No_Key_Found_Asynchronously(string key)
    {
        // Act
        bool removed = await _fixture.Cache.RemoveAsync(key);
        
        // Assert
        removed.Should().BeFalse();
    }

    internal class CacheValues
    {
        public string String { get; set; }
    
        public int Int { get; set; } 
    }
    
    internal class Cached
    {
        public string String { get; set; }
    
        public int Int { get; set; }

        public CacheValues Values { get; set; }
    }
}