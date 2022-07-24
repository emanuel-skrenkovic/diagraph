using Diagraph.Infrastructure.Dynamic.Extensions;
using Diagraph.Infrastructure.Integrations.Google;
using Diagraph.Infrastructure.Integrations.Google.InterModuleIntegration;
using Diagraph.Modules.Identity.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Identity.ExternalIntegrations.Google;

public class AccessTokenRefreshedHandler : INotificationHandler<AccessTokenRefreshedNotification>
{
    private readonly IdentityDbContext _context;
    
    public AccessTokenRefreshedHandler(IdentityDbContext context) => _context = context;
    
    public async Task Handle
    (
        AccessTokenRefreshedNotification notification, 
        CancellationToken cancellationToken
    )
    {
        External external = await _context
            .UserExternalIntegrations
            .SingleOrDefaultAsync(e => e.UserId == notification.UserId, cancellationToken);
        
        if (external is null) return;

        external.UpdateData<External, GoogleIntegrationInfo>
        (
            info => 
                info.AccessToken = info.AccessToken with { AccessToken = notification.AccessToken }
        );

        _context.Update(external);
        await _context.SaveChangesAsync(cancellationToken);
    }
}