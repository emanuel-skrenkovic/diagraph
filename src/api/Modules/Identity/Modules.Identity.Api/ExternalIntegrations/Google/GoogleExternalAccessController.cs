using System.Collections.Specialized;
using System.Web;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.Identity.Api.ExternalIntegrations.Google.Commands;
using Diagraph.Modules.Identity.Database;
using Diagraph.Modules.Identity.ExternalIntegrations;
using Diagraph.Modules.Identity.ExternalIntegrations.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Identity.Api.ExternalIntegrations.Google;

[ApiController]
[Route("auth/external-access/google")]
public class GoogleExternalAccessController : ControllerBase
{
    private readonly IdentityDbContext   _context;
    private readonly IUserContext        _userContext;
    private readonly GoogleConfiguration _configuration;

    public GoogleExternalAccessController
    (
        IdentityDbContext   context, 
        IUserContext        userContext,
        GoogleConfiguration configuration
    )
    {
        _context       = context;
        _userContext   = userContext;
        _configuration = configuration;
    }
    
    [HttpPost]
    [Route("scopes/request")]
    public IActionResult RequestGoogleScopesAccess
    (
        [FromBody] RequestGoogleScopesAccessCommand command
    )
    {
        if (command.Scopes?.Any() != true) return BadRequest();
        UriBuilder builder = new(_configuration.AuthUrl);
        
        NameValueCollection query = HttpUtility.ParseQueryString("");
        query.Add(OAuth2Constants.GrantType, OAuth2Constants.GrantTypes.AuthorizationCode);
        query.Add(OAuth2Constants.Scope, command.Scopes.Aggregate((acc, val) => $"{acc} {val}"));
        query.Add(OAuth2Constants.ClientId, _configuration.ClientId);
        query.Add(OAuth2Constants.ClientSecret, _configuration.ClientSecret);
        query.Add(OAuth2Constants.RedirectUri, _configuration.RedirectUrl);
        builder.Query = query.ToString()!;
       
        return Ok
        (
            new RequestGoogleScopesAccessResponse { RedirectUri = builder.ToString() }
        );
    }

    [HttpPut]
    [Route("scopes/confirm")]
    public async Task<IActionResult> ConfirmGoogleScopesAccess
    (
        [FromBody] ConfirmGoogleScopesAccessCommand command
    )
    {
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
                AuthUrl       = _configuration.AuthUrl,
                RedirectUrl   = _configuration.RedirectUrl,
                GrantedScopes = command.Scopes
            }
        );

        if (newIntegration)
        {
            _context.Add(external);
        }
        else
        {
            _context.Update(external); 
        }
        
        await _context.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpGet]
    [Route("scopes")]
    public async Task<IActionResult> RequestGoogleScopesAccess()
    {
        External external = await _context
            .UserExternalIntegrations
            .WithUser(_userContext.UserId)
            .FirstOrDefaultAsync(e => e.Provider == ExternalProvider.Google);

        return Ok
        (
            new { 
                GrantedScopes = external
                    ?.GetData<GoogleIntegrationInfo>()
                    .GrantedScopes ?? Array.Empty<string>()
            }
        );
    }
}