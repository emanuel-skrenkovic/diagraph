using Diagraph.Infrastructure.Auth;
using Diagraph.Modules.Identity.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diagraph.Modules.Identity.Api.Profiles;

[Authorize]
[ApiController]
[Route("my")]
public class MyController : ControllerBase
{
    private readonly IUserContext       _userContext;
    private readonly IdentityDbContext  _context;
    private readonly UserProfileManager _userProfileManager;

    public MyController
    (
        IUserContext       userContext, 
        IdentityDbContext  context, 
        UserProfileManager userProfileManager
    )
    {
        _userContext        = userContext;
        _context            = context;
        _userProfileManager = userProfileManager;
    }
    
    [HttpGet]
    [Route("profile")]
    public async Task<IActionResult> GetProfile()
    {
        UserProfile profile = await _userProfileManager.GetOrAddProfile
        (
            _userContext.UserId, 
            saveChanges: true
        );
        return Content(profile.Data);
    }

    [HttpPut]
    [Route("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] dynamic profileData)
    {
        UserProfile profile = await _userProfileManager.GetOrAddProfile(_userContext.UserId);
        profile.Data = profileData.ToString();

        _context.Update(profile);
        await _context.SaveChangesAsync();

        return Ok();
    }
}