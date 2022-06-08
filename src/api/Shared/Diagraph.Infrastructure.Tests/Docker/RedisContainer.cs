using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using Xunit;

namespace Diagraph.Infrastructure.Tests.Docker;

public class RedisContainer : IAsyncLifetime
{
    private readonly DockerContainer _container;
    public ConnectionMultiplexer     Multiplexer;

    private readonly Func<ConnectionMultiplexer> _multiplexerFactory;
    
    private const string ContainerName = "integration-tests.redis";
    private const string ImageName     = "redis:7.0.0-alpine";

    public RedisContainer(string connectionString)
    {
        string port = connectionString
            .Split(':')
            [1];

        _container = new DockerContainer
        (
            ContainerName, 
            ImageName, 
            new(), 
            new() { ["6379"] = port }
        )
        {
            CheckStatus = CheckConnection
        };

        _multiplexerFactory = () => ConnectionMultiplexer.Connect(connectionString);
    }
    
    private Task<bool> CheckConnection()
    {
        Multiplexer = _multiplexerFactory();
        if (Multiplexer == null) return Task.FromResult(false);

        return Task.FromResult(Multiplexer.IsConnected);
    }
    
    #region IAsyncLifetime

    public Task InitializeAsync() => _container.InitializeAsync();
    
    public async Task DisposeAsync() 
    {
        Multiplexer?.Dispose();
        await _container.DisposeAsync();
    }
    
    #endregion
}