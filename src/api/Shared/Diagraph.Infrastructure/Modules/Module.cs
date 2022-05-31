using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diagraph.Infrastructure.Modules;

public abstract class Module
{
    public abstract string ModuleName { get; }
    
    public void Load(IServiceCollection services, string environment = null)
        => RegisterServices(LoadConfiguration(environment), services);

    public void Load(IServiceCollection services, IConfiguration configuration)
        => RegisterServices(configuration, services);
    
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
    
    protected abstract void RegisterServices(IConfiguration configuration, IServiceCollection services);
}
