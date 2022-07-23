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
        if (configuration is null) return services;
        
        services.TryAddSingleton
        (
            ConnectionMultiplexer.Connect
            (
                configuration.ConnectionString, 
                options => options.DefaultDatabase = configuration.Database ?? 0
            )
        );

        services.TryAddScoped<ICache, RedisCache>();

        return services;
    }
}