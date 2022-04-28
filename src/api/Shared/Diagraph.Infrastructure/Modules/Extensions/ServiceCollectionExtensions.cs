using Microsoft.EntityFrameworkCore;
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
        module.Load(services, environment); // TODO: fix
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
        string connectionString,
        string migrationsAssembly = null
    ) where TContext : DbContext
    {
         services.AddDbContext<TContext>
         (
             options => options.UseNpgsql
             (
                 connectionString,
                 migrationsAssembly is null 
                     ? _ => {} 
                     : b => b.MigrationsAssembly(migrationsAssembly)
             )
         );

         return services;
    }
}