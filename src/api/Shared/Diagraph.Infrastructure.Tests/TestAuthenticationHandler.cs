using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Diagraph.Infrastructure.Tests;

public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
{
    public const string AuthScheme = "Integration-Test";
    
    public TestAuthenticationHandler
    (
        IOptionsMonitor<TestAuthenticationOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock
    ) 
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        AuthenticationTicket authenticationTicket = new
        (
            new ClaimsPrincipal(Options.Identity),
            new AuthenticationProperties(),
            AuthScheme
        );

        return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
    }
}

public static class TestAuthenticationExtensions
{
    public static AuthenticationBuilder AddTestAuth(this AuthenticationBuilder builder, Action<TestAuthenticationOptions> configureOptions)
    {
        return builder.AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>
        (
            TestAuthenticationHandler.AuthScheme, 
            TestAuthenticationHandler.AuthScheme, 
            configureOptions
        );
    }
}

public class TestAuthenticationOptions : AuthenticationSchemeOptions
{
    public virtual ClaimsIdentity Identity => new(new Claim[]
    {
        new
        (
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", 
            "bb532089-d6ce-4505-a593-5e10c1668bb8"
        )
    }, "test");
}