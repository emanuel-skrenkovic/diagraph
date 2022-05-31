using Diagraph.Infrastructure;
using Diagraph.Infrastructure.Dynamic;
using Diagraph.Infrastructure.Integrations;

namespace Diagraph.Modules.Identity.ExternalIntegrations;

public class External : IUserRelated, IDynamicDataContainer
{
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public ExternalProvider Provider { get; set; }
    
    public string Data { get; set; }

    public static External Create(Guid userId, ExternalProvider provider)
    {
        if (userId == default) 
            throw new ArgumentException($"{nameof(userId)} cannot be equal to the default value.");

        return new()
        {
            UserId   = userId,
            Provider = provider
        };
    }
}