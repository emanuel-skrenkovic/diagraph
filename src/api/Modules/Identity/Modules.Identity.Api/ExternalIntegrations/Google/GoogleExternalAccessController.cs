using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Auth.OAuth2;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.Dynamic.Extensions;
using Diagraph.Infrastructure.Integrations;
using Diagraph.Infrastructure.Integrations.Google;
using Diagraph.Modules.Identity.Api.ExternalIntegrations.Google.Commands;
using Diagraph.Modules.Identity.Database;
using Diagraph.Modules.Identity.ExternalIntegrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Identity.Api.ExternalIntegrations.Google;

[ApiController]
[Route("auth/external-access/google")]
public class GoogleExternalAccessController : ControllerBase
{
    private readonly IdentityDbContext _context;
    private readonly IUserContext      _userContext;
    private readonly GoogleAuthorizer  _authorizer;

    public GoogleExternalAccessController
    (
        IdentityDbContext context, 
        IUserContext      userContext,
        GoogleAuthorizer  authorizer
    )
    {
        _context     = context;
        _userContext = userContext;
        _authorizer  = authorizer;
    }
    
    [HttpGet]
    [Route("scopes/request")]
    public async Task<IActionResult> RequestGoogleScopesAccess([FromQuery] string redirectUri) 
        => Ok
        (
            new RequestGoogleScopesAccessResponse
            {
                RedirectUri = _authorizer.Scopes.GenerateRequestsScopesUrl
                (
                    await _authorizer.Scopes.RequestRequiredAsync("tasks", "v1"),
                    redirectUri
                )
            }
        );

    // TODO: refactor the entire procedure.
    [HttpPut]
    [Route("scopes/confirm")]
    public async Task<IActionResult> ConfirmGoogleScopesAccess
    (
        [FromBody] ConfirmGoogleScopesAccessCommand command
    )
    {
        OAuth2TokenResponse tokenResponse = await _authorizer.AuthFlow.ExecuteAsync
        (
            command.Code, 
            command.RedirectUri
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
                GrantedScopes = command.Scope,
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

        await _context.SaveChangesAsync();
        await _authorizer.InitializeSessionDataAsync
        (
            tokenResponse.AccessToken,
            tokenResponse.ExpiresIn,
            tokenResponse.RefreshToken
        );
        
        return Ok();
    }
    
    [HttpGet]
    [Route("scopes")]
    public async Task<IActionResult> RequestAvailableScopes()
    {
        External external = await _context
            .UserExternalIntegrations
            .WithUser(_userContext.UserId)
            .FirstOrDefaultAsync(e => e.Provider == ExternalProvider.Google);

        return Ok
        (
            new 
            { 
                GrantedScopes = external
                    ?.GetData<GoogleIntegrationInfo>()
                    .GrantedScopes ?? Array.Empty<string>()
            }
        );
    }
}