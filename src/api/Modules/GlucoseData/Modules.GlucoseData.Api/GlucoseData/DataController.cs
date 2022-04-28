using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.GlucoseData.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.GlucoseData.Api.GlucoseData;

[Authorize]
[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase
{
    private readonly IUserContext         _userContext;
    private readonly GlucoseDataDbContext _context;

    public DataController
    (
        IUserContext         userContext,
        GlucoseDataDbContext context
    )
    {
        _userContext = userContext;
        _context     = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetData([FromQuery] DateTime from, [FromQuery] DateTime to)
        => Ok
        (
            await _context
                .GlucoseMeasurements
                .WithUser(_userContext.UserId)
                .Where(m => m.TakenAt >= from && m.TakenAt < to&& m.Level > 0)
                .OrderBy(m => m.TakenAt)
                .ToListAsync()
        );
    

}