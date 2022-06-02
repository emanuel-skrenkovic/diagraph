using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Auth.OAuth2;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.Dynamic.Extensions;
using Diagraph.Infrastructure.Integrations;
using Diagraph.Infrastructure.Integrations.Google;
using Diagraph.Modules.Identity.Api.ExternalIntegrations.Google.Commands;
using Diagraph.Modules.Identity.Database;
using Diagraph.Modules.Identity.ExternalIntegrations;
using FastEndpoints;

namespace Diagraph.Modules.Identity.Api.ExternalIntegrations.Google;

public class ConfirmGoogleTasksScopesEndpoint : Endpoint<ConfirmGoogleTasksScopesCommand>
{
    private readonly IdentityDbContext _context;
    private readonly IUserContext      _userContext;
    private readonly GoogleAuthorizer  _authorizer;

    public ConfirmGoogleTasksScopesEndpoint
    (
        IdentityDbContext context, 
        IUserContext userContext, 
        GoogleAuthorizer authorizer
    )
    {
        _context     = context;
        _userContext = userContext;
        _authorizer  = authorizer;
    }
    
    public override void Configure()
        => Put("auth/external-access/google/scopes/confirm");

    public override async Task HandleAsync(ConfirmGoogleTasksScopesCommand req, CancellationToken ct)
    {
        OAuth2TokenResponse tokenResponse = await _authorizer.AuthFlow.ExecuteAsync
        (
            req.Code, 
            req.RedirectUri
        );
        
        External external = await _context.GetOrAddAsync
        (
            e => e.UserId == _userContext.UserId && e.Provider == ExternalProvider.Google,
            External.Create(_userContext.UserId, ExternalProvider.Google)
        );
        external.SetData
        (
            new GoogleIntegrationInfo
            {
                GrantedScopes = req.Scope,
                AccessToken   = new TokenData(tokenResponse.AccessToken, tokenResponse.ExpiresIn),
                RefreshToken  = tokenResponse.RefreshToken
            }
        );

        UserProfile profile = await _context.GetOrAddAsync
        (
            p => p.UserId == _userContext.UserId, 
            UserProfile.Create(_userContext.UserId)
        );
        profile.AddOrUpdateData<UserProfile, Dictionary<string, object>>
        (
            data => data["googleIntegration"] = true // UGLY
        );

        await _context.SaveChangesAsync(ct);
        await _authorizer.InitializeSessionDataAsync
        (
            tokenResponse.AccessToken,
            tokenResponse.ExpiresIn,
            tokenResponse.RefreshToken
        );

        await SendOkAsync(ct);
    }
}