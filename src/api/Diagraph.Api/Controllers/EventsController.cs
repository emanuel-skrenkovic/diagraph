using Diagraph.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
{
    private readonly DiagraphDbContext _context;

    public EventsController(DiagraphDbContext context) 
        => _context = context;

    [HttpGet]
    [Route("meals")]
    public async Task<IActionResult> GetMeals
    (
        [FromQuery] DateTime from,
        [FromQuery] DateTime to
    )
        => Ok
        (
            await _context
                .Meals
                .Where(m => m.OccurredAtUtc >= from && m.OccurredAtUtc < to)
                .ToListAsync()
        );
    
    [HttpGet]
    [Route("insulin-applications")]
    public async Task<IActionResult> GetInsulinApplications
    (
        [FromQuery] DateTime from,
        [FromQuery] DateTime to
    )
        => Ok
        (
            await _context
                .InsulinApplications
                .Where(m => m.OccurredAtUtc >= from && m.OccurredAtUtc < to)
                .ToListAsync()
        );

    [HttpGet]
    [Route("misc")]
    public async Task<IActionResult> GetMiscellaneousEvents
    (
        [FromQuery] DateTime from,
        [FromQuery] DateTime to
    )
        => Ok
        (
            await _context.MiscellanousEvents
                .Where(e => e.OccurredAtUtc >= from && e.OccurredAtUtc < to)
                .ToListAsync()
        );
}