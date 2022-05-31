using System.Security.Principal;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.Emails;
using Diagraph.Infrastructure.ErrorHandling;
using Diagraph.Infrastructure.Integrations;
using Diagraph.Infrastructure.Sessions;
using Diagraph.Modules.Identity.Api.Auth.Commands;
using Diagraph.Modules.Identity.Api.Extensions;
using Diagraph.Modules.Identity.Database;
using Diagraph.Modules.Identity.ExternalIntegrations;
using Diagraph.Modules.Identity.Registration;
using Diagraph.Modules.Identity.ValueObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiagraphUser = Diagraph.Modules.Identity.User;

namespace Diagraph.Modules.Identity.Api.Auth;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private const string AuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    
    private readonly IdentityDbContext           _context;
    private readonly PasswordTool                _passwordTool;
    private readonly UserConfirmation            _userConfirmation;
    private readonly SessionManager              _sessionManager;
    private readonly IIntegrationSessionProvider _integration;

    public AuthController
    (
        IdentityDbContext           context, 
        PasswordTool                passwordTool, 
        UserConfirmation            userConfirmation,
        SessionManager              sessionManager,
        IIntegrationSessionProvider integration
    )
    {
        _context          = context;
        _passwordTool     = passwordTool;
        _userConfirmation = userConfirmation;
        _sessionManager   = sessionManager;
        _integration      = integration;
    }

    [HttpGet]
    [Route("session")]
    public async Task<IActionResult> GetSession()
    {
        IIdentity identity = HttpContext.User.Identity;
        
        if (identity?.IsAuthenticated != true)                             return StatusCode(403);
        if (!await _context.Users.AnyAsync(u => u.Email == identity.Name)) return StatusCode(403);
        
        return Ok
        (
            new SessionInfo
            {
                UserName           = identity.Name,
                IsAuthenticated    = identity.IsAuthenticated,
                AuthenticationType = identity.AuthenticationType
            }
        );
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterCommand command)
    {
        if (await _context.Users.AnyAsync(u => u.Email == command.Email))
        {
            // Just return ok if the user already exists. If it's a valid request,
            // the user will check their email.
            return Ok();
        }

        User user = DiagraphUser.Create
        (
            EmailAddress.Create(command.Email),
            Password.Create(command.Password),
            _passwordTool
        );
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        // TODO: read from database and send on timer, akin to outbox?
        await _userConfirmation.SendConfirmationEmailAsync
        (
            user, 
            new Uri($"{Request.Scheme}://{Request.Host}")
        );
        
        return Ok();
    }

    [HttpGet]
    [Route("register/confirm")]
    public async Task<IActionResult> ConfirmRegistration([FromQuery] string token)
    {
        Result confirmationResult = await _userConfirmation.ValidateUserConfirmationAsync(token);

        return confirmationResult.Match<IActionResult>
        (
            Ok, 
            _ => BadRequest("Failed to validate confirmation token.")
        );
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == command.Email);
        
        if (user == null)         return Unauthorized("Invalid user or password.");
        if (user.Locked)          return Unauthorized("User is locked.");
        if (!user.EmailConfirmed) return Unauthorized("User email is not confirmed.");

        AuthResult authResult = user.Authenticate(command.Password, _passwordTool);

        _context.Update(user);
        await _context.SaveChangesAsync();

        if (!authResult.Authenticated) return Unauthorized(authResult.Reason);

        await HttpContext.SignInUserAsync(user, AuthScheme);

        List<External> externals = await _context
            .UserExternalIntegrations
            .WithUser(user.Id)
            .ToListAsync();

        await Task.WhenAll
        (
            externals
                .Select(e => _integration.Get(e.Provider)?.InitializeAsync(e) ?? Task.CompletedTask)
                .ToList()
        );
        
        return Ok();
    }

    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        await _sessionManager.ClearAsync();
        await HttpContext.SignOutAsync(AuthScheme);
        return Ok();
    }
}