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
        ApplicationPartManager partManager,
        string environment = null
    ) 
        where T: Module, new()
    {
        partManager.ApplicationParts.Add
        (
            new AssemblyPart(typeof(T).Assembly)
        );
        
        T module = new();
        module.Load(partManager, services, environment); // TODO: fix

        services.AddSingleton(module);
        
        return services;
    }
    
    public static IServiceCollection LoadModule<T>
    (
        this IServiceCollection services, 
        ApplicationPartManager  partManager,
        IConfiguration          configuration
    )
        where T: Module, new()
    {
        partManager.ApplicationParts.Add
        (
            new AssemblyPart(typeof(T).Assembly)
        );
        
        T module = new();
        module.Load(partManager, services, configuration);
        
        services.AddSingleton(module);
        
        return services;
    }

    public static IServiceCollection Clone(this IServiceCollection services)
    {
        IServiceCollection clonedServices = new ServiceCollection();
        
        foreach (ServiceDescriptor service in services)
            clonedServices.Add(service);
        
        return clonedServices;
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