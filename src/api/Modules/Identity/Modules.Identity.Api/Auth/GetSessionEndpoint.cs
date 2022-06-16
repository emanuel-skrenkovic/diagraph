using System.Security.Principal;
using Diagraph.Modules.Identity.Api.Auth.Contracts;
using Diagraph.Modules.Identity.Database;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Identity.Api.Auth;

public class GetSessionEndpoint : EndpointWithoutRequest
{
    private readonly IdentityDbContext _context;

    public GetSessionEndpoint(IdentityDbContext context)
        => _context = context;
    
    public override void Configure() => Get("auth/session");

    public override async Task HandleAsync(CancellationToken ct)
    {
        IIdentity identity = HttpContext.User.Identity;

        if (identity?.IsAuthenticated != true)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        if (!await _context.Users.AnyAsync(u => u.Email == identity.Name, ct))
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        await SendOkAsync
        (
            new SessionInfo
            {
                UserName           = identity.Name,
                IsAuthenticated    = identity.IsAuthenticated,
                AuthenticationType = identity.AuthenticationType
            },
            ct
        );
    }
}