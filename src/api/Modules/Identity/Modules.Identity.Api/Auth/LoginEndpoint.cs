using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.Integrations;
using Diagraph.Infrastructure.Sessions;
using Diagraph.Modules.Identity.Api.Auth.Contracts;
using Diagraph.Modules.Identity.Api.Extensions;
using Diagraph.Modules.Identity.Database;
using Diagraph.Modules.Identity.ExternalIntegrations;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Identity.Api.Auth;

public class LoginEndpoint : Endpoint<LoginCommand, LoginResult>
{
    private const string AuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    
    private readonly IdentityDbContext           _context;
    private readonly PasswordTool                _passwordTool;
    private readonly SessionManager              _sessionManager;
    private readonly IIntegrationSessionProvider _integration;

    public LoginEndpoint
    (
        IdentityDbContext           context, 
        PasswordTool                passwordTool, 
        SessionManager              sessionManager,
        IIntegrationSessionProvider integration
    )
    {
        _context        = context;
        _passwordTool   = passwordTool;
        _sessionManager = sessionManager;
        _integration    = integration;
    }

    public override void Configure()
    {
        Post("auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginCommand req, CancellationToken ct)
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == req.Email, ct);

        string unauthorizedReason = null;
        
        if      (user == null)         unauthorizedReason = "Invalid user or password.";
        else if (user.Locked)          unauthorizedReason = "User is locked.";
        else if (!user.EmailConfirmed) unauthorizedReason = "User email is not confirmed";
        
        if (unauthorizedReason is not null)
        {
            await Unauthorized(unauthorizedReason, ct);
            return;
        }

        AuthResult authResult = user.Authenticate(req.Password, _passwordTool);

        _context.Update(user); // TODO: probably not necessary.
        await _context.SaveChangesAsync(ct);

        if (!authResult.Authenticated)
        {
            await Unauthorized(authResult.Reason, ct); 
            return;
        }

        await HttpContext.SignInUserAsync(user, AuthScheme);
        await _sessionManager.SaveAsync(SessionConstants.UserId, user.Id);

        List<External> externals = await _context
            .UserExternalIntegrations
            .WithUser(user.Id)
            .ToListAsync(ct);

        if (externals.Any())
        {
            await Task.WhenAll
            (
                externals
                    .Select(e => _integration.Get(e.Provider)?.InitializeAsync(e) ?? Task.CompletedTask)
                    .ToList()
            );
        }

        await SendOkAsync(ct);
    }

    private Task Unauthorized(string reason, CancellationToken ct) 
        => SendAsync(new LoginResult { Reason = reason }, 401, ct);
}