using System.Threading.Tasks;
using Diagraph.Infrastructure.Cache;
using Diagraph.Infrastructure.Cache.Redis;
using Diagraph.Infrastructure.Tests.Docker;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Diagraph.Infrastructure.Tests;

public class RedisModuleFixture : IAsyncLifetime
{
    public readonly RedisContainer Redis;

    public ICache Cache => new RedisCache(Redis.Multiplexer);

    public RedisModuleFixture(string moduleName)
    {
        IConfiguration configuration = new ConfigurationManager()
            .AddJsonFile($"module.{moduleName}.integration-test.json")
            .Build();

        Redis = new RedisContainer(configuration["RedisConfiguration:ConnectionString"]);
    }

    public Task InitializeAsync() => Redis.InitializeAsync();

    public Task DisposeAsync() => Redis.DisposeAsync();
}