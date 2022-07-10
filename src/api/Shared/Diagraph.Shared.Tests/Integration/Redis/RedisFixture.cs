using Diagraph.Infrastructure.Tests;

namespace Diagraph.Shared.Tests.Integration.Redis;

public class RedisFixture : RedisModuleFixture
{
    public RedisFixture() : base("shared") { }
}