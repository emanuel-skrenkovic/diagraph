using Diagraph.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;

namespace Diagraph.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ImportsController : ControllerBase
{
    private readonly DiagraphDbContext _context;

    public ImportsController(DiagraphDbContext context)
        => _context = context;
    
    [HttpPost]
    public Task<IActionResult> ImportData()
    {
        throw new NotImplementedException();
    }
}