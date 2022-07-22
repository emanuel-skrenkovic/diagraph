using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diagraph.Infrastructure.Modules;

public abstract class Module
{
    public abstract string ModuleName { get; }
    
    public void Load(ApplicationPartManager partManager, IServiceCollection services, string environment = null)
        => RegisterServices(partManager, LoadConfiguration(environment), services);

    public void Load(ApplicationPartManager partManager, IServiceCollection services, IConfiguration configuration)
        => RegisterServices(partManager, configuration, services);
    
    protected IConfiguration LoadConfiguration(string environment = null)
    {
        string envPart = string.IsNullOrWhiteSpace(environment) ? "" : '.' + environment.ToLower();
        string configurationFile = 
            $"module.{ModuleName}{envPart}.json";

        string path = AppDomain.CurrentDomain.BaseDirectory;
        
        return new ConfigurationManager()
            .AddJsonFile($"{path}/{configurationFile}", optional: true)
            .Build();
    }
    
    protected abstract void RegisterServices
    (
        ApplicationPartManager partManager, 
        IConfiguration configuration, 
        IServiceCollection services
    );
}
