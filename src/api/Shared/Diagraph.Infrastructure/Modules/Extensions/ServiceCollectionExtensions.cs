using Diagraph.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diagraph.Infrastructure.Modules.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection LoadModule<T>
    (
        this IServiceCollection services, 
        string environment = null
    ) 
        where T: Module, new()
    {
        T module = new();
        module.Load(services, environment);
        
        return services;
    }
    
    public static IServiceCollection LoadModule<T>
    (
        this IServiceCollection services, 
        IConfiguration          configuration
    )
        where T: Module, new()
    {
        T module = new();
        module.Load(services, configuration);
        return services;
    }
    
    public static IServiceCollection AddPostgres<TContext>
    (
        this IServiceCollection services,
        DatabaseConfiguration configuration
    ) where TContext : DbContext
    {
         services.AddDbContext<TContext>
         (
             options => options.UseNpgsql
             (
                 configuration.ConnectionString,
                 configuration.MigrationsAssembly is null 
                     ? _ => {} 
                     : b => b.MigrationsAssembly(configuration.MigrationsAssembly)
             )
         );

         return services;
    }
}