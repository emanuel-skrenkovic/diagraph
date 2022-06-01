using Diagraph.Infrastructure.Sessions;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Diagraph.Modules.Identity.Api.Auth;

public class LogoutEndpoint : EndpointWithoutRequest
{
    private readonly SessionManager _sessionManager;

    public LogoutEndpoint(SessionManager sessionManager)
        => _sessionManager = sessionManager;
    
    public override void Configure()
        => Post("auth/logout");

    public override async Task HandleAsync(CancellationToken ct)
    {
        await _sessionManager.ClearAsync();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await SendOkAsync(ct);
    }
}