using Diagraph.Infrastructure.Auth;
using Diagraph.Modules.Identity.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Identity.Api.Profiles;

[Authorize]
[ApiController]
[Route("my")]
public class MyController : ControllerBase
{
    private readonly IUserContext _userContext;
    private readonly IdentityDbContext _context;

    public MyController(IUserContext userContext, IdentityDbContext context)
    {
        _userContext = userContext;
        _context     = context;      
    }
    
    [HttpGet]
    [Route("profile")]
    public async Task<IActionResult> GetProfile()
    {
        UserProfile profile = await UserProfile();
        return Content(profile.Data);
    }

    [HttpPut]
    [Route("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] dynamic profileData)
    {
        UserProfile profile = await UserProfile();
        profile.Data = profileData.ToString();

        _context.Update(profile);
        await _context.SaveChangesAsync();

        return Ok();
    }

    private async Task<UserProfile> UserProfile()
    {
        Guid userId = _userContext.UserId;
        
        UserProfile profile = await _context
            .Profiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
        
        if (profile == null)
        {
            profile = _context
                .Profiles
                .Add(new UserProfile { UserId = userId })
                .Entity;
            
            await _context.SaveChangesAsync();
        }
        
        return profile;
    }
}