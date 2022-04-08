using System.Security.Claims;
using System.Security.Principal;
using Diagraph.Api.Models;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Emails;
using Diagraph.Infrastructure.ErrorHandling;
using Diagraph.Infrastructure.Models;
using Diagraph.Infrastructure.Models.ValueObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private const string AuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    
    private readonly DiagraphDbContext       _context;
    private readonly PasswordTool            _passwordTool;
    private readonly UserConfirmationService _userConfirmation;

    public AuthController(DiagraphDbContext context, PasswordTool passwordTool, UserConfirmationService userConfirmation)
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
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(); // TODO: think about this
        }

        User user = Diagraph.Infrastructure.Models.User.Create
        (
            EmailAddress.Create(request.Email),
            Password.Create(request.Password),
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

        // TODO: check if correct.
        return confirmationResult.IsOk 
            ? Redirect("/login") 
            : BadRequest("Failed to validate confirmation token.");
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        
        if (user == null)         return Unauthorized("Invalid user or password.");
        if (user.Locked)          return Unauthorized("User is locked.");
        if (!user.EmailConfirmed) return Unauthorized("User email is not confirmed.");

        if (!_passwordTool.Compare(user.PasswordHash, request.Password))
        {
            user.Locked = ++user.UnsuccessfulLoginAttempts >= 3;
            if (user.Locked) user.SecurityStamp = Guid.NewGuid();

            _context.Update(user);
            await _context.SaveChangesAsync();
            
            return Unauthorized(user.Locked ? "Account has been locked." : "Invalid user or password.");
        }

        ClaimsIdentity identity = new ClaimsIdentity(AuthScheme);
        identity.AddClaim(new(ClaimTypes.Name, user.Email)); // TODO
        identity.AddClaim(new(ClaimTypes.Email, user.Email)); // TODO
        identity.AddClaim(new(ClaimTypes.NameIdentifier, user.Id.ToString()));
        
        await HttpContext.SignInAsync(AuthScheme, new(identity));
        
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