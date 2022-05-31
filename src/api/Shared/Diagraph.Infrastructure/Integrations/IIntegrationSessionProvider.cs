namespace Diagraph.Infrastructure.Integrations;

public interface IIntegrationSessionProvider
{
    public IIntegrationSession Get(ExternalProvider provider);
}