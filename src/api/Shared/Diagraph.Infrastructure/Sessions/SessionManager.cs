using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Diagraph.Infrastructure.Sessions;

public class SessionManager
{
    private readonly IHttpContextAccessor _contextAccessor;

    public SessionManager(IHttpContextAccessor contextAccessor)
        => _contextAccessor = contextAccessor;
   
    public async Task<T> GetAsync<T>(string key)
    {
        ISession session = await LoadSessionAsync();

        string stringValue = session.GetString(key);
        if (stringValue is null) return default;

        return JsonSerializer.Deserialize<T>(stringValue);
    }

    public async Task SaveAsync<T>(string key, T value)
    {
        ISession session = await LoadSessionAsync();
        session.SetString(key, JsonSerializer.Serialize(value));
        
        await session.CommitAsync();
    }

    public async Task RemoveAsync(string key)
    {
        ISession session = await LoadSessionAsync();
        session.Remove(key);
        
        await session.CommitAsync();
    }

    public async Task ClearAsync()
    {
        ISession session = await LoadSessionAsync();
        session.Clear();
        await session.CommitAsync();
    }

    private async Task<ISession> LoadSessionAsync()
    {
        ISession session = _contextAccessor?.HttpContext?.Session;
        if (session is null) throw new InvalidOperationException("Session is not available.");
        
        await session.LoadAsync();

        return session;
    }
}