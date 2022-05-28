using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.Dynamic.Extensions;
using Diagraph.Modules.Identity.Api.ExternalIntegrations.Google.Commands;
using Diagraph.Modules.Identity.Database;
using Diagraph.Modules.Identity.ExternalIntegrations;
using Diagraph.Modules.Identity.ExternalIntegrations.Google;
using Diagraph.Modules.Identity.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IAuthorizationCodeFlow = Diagraph.Modules.Identity.OAuth2.IAuthorizationCodeFlow;

namespace Diagraph.Modules.Identity.Api.ExternalIntegrations.Google;

[ApiController]
[Route("auth/external-access/google")]
public class GoogleExternalAccessController : ControllerBase
{
    private readonly IdentityDbContext      _context;
    private readonly IUserContext           _userContext;
    private readonly GoogleScopes           _scopes;
    private readonly UserProfileManager     _userProfileManager;
    private readonly IAuthorizationCodeFlow _authFlow;

    public GoogleExternalAccessController
    (
        IdentityDbContext      context, 
        IUserContext           userContext,
        GoogleScopes           scopes,
        UserProfileManager     userProfileManager,
        IAuthorizationCodeFlow authFlow
    )
    {
        _context            = context;
        _userContext        = userContext;
        _scopes             = scopes;
        _userProfileManager = userProfileManager;
        _authFlow           = authFlow;
    }
    
    [HttpGet]
    [Route("scopes/request")]
    public async Task<IActionResult> RequestGoogleScopesAccess([FromQuery] string redirectUri) 
        => Ok
        (
            new RequestGoogleScopesAccessResponse
            {
                RedirectUri = _scopes.GenerateRequestsScopesUrl
                (
                    await _scopes.RequestRequiredAsync("tasks", "v1"),
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
        OAuth2TokenResponse tokenResponse = await _authFlow.ExecuteAsync
        (
            command.Code, 
            command.RedirectUri
        );
        
        External external = await _context
            .UserExternalIntegrations
            .WithUser(_userContext.UserId)
            .FirstOrDefaultAsync(e => e.Provider == ExternalProvider.Google) ;

        bool newIntegration = external is null;

        if (newIntegration)
        {
            external = new()
            {
                UserId   = _userContext.UserId,
                Provider = ExternalProvider.Google
            }; 
        }
        
        external.SetData
        (
            new GoogleIntegrationInfo
            {
                GrantedScopes = command.Scope,
                RefreshToken  = tokenResponse.RefreshToken
            }
        );

        if (newIntegration) _context.Add(external);
        else                _context.Update(external);

        UserProfile profile = await _userProfileManager.GetOrAddProfile(_userContext.UserId);
        profile.UpdateData<UserProfile, Dictionary<string, object>>
        (
            data => data["googleIntegration"] = true
        );

        await _context.SaveChangesAsync();
        
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