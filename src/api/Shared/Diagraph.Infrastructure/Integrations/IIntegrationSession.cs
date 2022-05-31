using Diagraph.Infrastructure.Dynamic;

namespace Diagraph.Infrastructure.Integrations;

public interface IIntegrationSession
{
    ExternalProvider Provider { get; }
    
    Task InitializeAsync(IDynamicDataContainer dataContainer);

    Task TerminateAsync();
}