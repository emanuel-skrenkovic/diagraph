using System.Security.Claims;
using System.Security.Principal;
using Diagraph.Api.Models;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Models;
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
    private const string LoginFailReason = "Invalid user or password.";
    
    private readonly DiagraphDbContext _context;
    private readonly PasswordTool _passwordTool;

    public AuthController(DiagraphDbContext context, PasswordTool passwordTool)
    {
        _context      = context;
        _passwordTool = passwordTool;
    }

    [HttpGet]
    [Route("session")]
    public IActionResult GetSession()
    {
        if (HttpContext.User.Identity?.IsAuthenticated != true) return Forbid();

        IIdentity identity = HttpContext.User.Identity;
        
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
            return BadRequest();
        }

        _context.Users.Add(new()
        {
            Id           = Guid.NewGuid(),
            UserName     = request.UserName,
            Email        = request.Email,
            PasswordHash = _passwordTool.Hash(request.Password)
        });
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return Unauthorized(LoginFailReason);

        if (!_passwordTool.Compare(user.PasswordHash, request.Password))
        {
            return Unauthorized(LoginFailReason);
        }

        ClaimsIdentity identity = new ClaimsIdentity(AuthScheme);
        identity.AddClaim
        (
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        );
        identity.AddClaim
        (
            new Claim(ClaimTypes.Name, user.Email) // TODO: username
        );
        
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