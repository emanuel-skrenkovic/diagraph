namespace Diagraph.Infrastructure.Integrations;

public class IntegrationSessionProvider : IIntegrationSessionProvider
{
    private readonly IDictionary<ExternalProvider, IIntegrationSession> _sessions;

    public IntegrationSessionProvider(IEnumerable<IIntegrationSession> sessions)
        => _sessions = sessions.ToDictionary(s => s.Provider, s => s);
    
    public IIntegrationSession Get(ExternalProvider provider) 
        => _sessions.TryGetValue(provider, out IIntegrationSession session) 
            ? session 
            : null;
}