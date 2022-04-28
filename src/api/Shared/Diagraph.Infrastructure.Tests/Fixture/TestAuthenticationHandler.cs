using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Diagraph.Infrastructure.Tests.Fixture;

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthScheme = "Integration-Test";
    
    public TestAuthenticationHandler
    (
        IOptionsMonitor<AuthenticationSchemeOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock
    ) 
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Claim[] claims              = { new(ClaimTypes.Name, "Test user") };
        ClaimsIdentity identity     = new ClaimsIdentity(claims, AuthScheme);
        ClaimsPrincipal principal   = new ClaimsPrincipal(identity);
        AuthenticationTicket ticket = new AuthenticationTicket(principal, AuthScheme);

        return Task.FromResult
        (
            AuthenticateResult.Success(ticket)
        );
    }
}