using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.Identity.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diagraph.Modules.Identity.Api.Profiles;

[Authorize]
[ApiController]
[Route("my")]
public class MyController : ControllerBase
{
    private readonly IUserContext      _userContext;
    private readonly IdentityDbContext _context;

    public MyController
    (
        IUserContext      userContext, 
        IdentityDbContext context
    )
    {
        _userContext        = userContext;
        _context            = context;
    }
    
    [HttpGet]
    [Route("profile")]
    public async Task<IActionResult> GetProfile()
    {
        UserProfile profile = await _context.GetOrAddAsync
        (
            p => p.UserId == _userContext.UserId, 
            UserProfile.Create(_userContext.UserId)
        );

        await _context.SaveChangesAsync();
        
        return Content(profile.Data);
    }

    [HttpPut]
    [Route("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] dynamic profileData)
    {
        UserProfile profile = await _context.GetOrAddAsync
        (
            p => p.UserId == _userContext.UserId,
            UserProfile.Create(_userContext.UserId)
        );
        
        profile.Data = profileData.ToString();

        _context.Update(profile);
        await _context.SaveChangesAsync();

        return Ok();
    }
}