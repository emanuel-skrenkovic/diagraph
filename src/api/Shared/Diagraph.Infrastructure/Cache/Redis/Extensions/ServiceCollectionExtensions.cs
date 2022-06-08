using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace Diagraph.Infrastructure.Cache.Redis.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedis
    (
        this IServiceCollection services, 
        RedisConfiguration configuration
    )
    {
        services.TryAddSingleton
        (
            ConnectionMultiplexer.Connect
            (
                configuration.ConnectionString, 
                // TODO: need to be able to have database per module
                options => options.DefaultDatabase = configuration.Database ?? 0
            )
        );

        services.TryAddScoped<ICache, RedisCache>();

        return services;
    }
}