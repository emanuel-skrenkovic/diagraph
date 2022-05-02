using System.Security.Principal;
using Diagraph.Infrastructure.Emails;
using Diagraph.Infrastructure.ErrorHandling;
using Diagraph.Modules.Identity.Api.Auth.Commands;
using Diagraph.Modules.Identity.Api.Extensions;
using Diagraph.Modules.Identity.Database;
using Diagraph.Modules.Identity.Registration;
using Diagraph.Modules.Identity.ValueObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Identity.Api.Auth;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private const string AuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    
    private readonly IdentityDbContext _context;
    private readonly PasswordTool      _passwordTool;
    private readonly UserConfirmation  _userConfirmation;

    public AuthController
    (
        IdentityDbContext context, 
        PasswordTool passwordTool, 
        UserConfirmation userConfirmation
    )
    {
        _context          = context;
        _passwordTool     = passwordTool;
        _userConfirmation = userConfirmation;
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

        User user = Diagraph.Modules.Identity.User.Create
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
        return Ok();
    }

    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(AuthScheme);
        return Ok();
    }
}