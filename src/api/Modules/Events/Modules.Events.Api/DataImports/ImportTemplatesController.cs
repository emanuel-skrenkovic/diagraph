using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.DataImports;

[Authorize]
[ApiController]
[Route("events/import-templates")]
public class ImportTemplatesController : ControllerBase
{
    private readonly EventsDbContext _context;
    private readonly IUserContext      _userContext;
 
    public ImportTemplatesController(EventsDbContext context, IUserContext userContext)
    {
        _context     = context;
        _userContext = userContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetImportTemplates()
        => Ok
        (
            await _context
                .Templates
                .WithUser(_userContext.UserId)
                .ToListAsync()
        );

    [HttpPost]
    public async Task<IActionResult> CreateImportTemplate(ImportTemplate command)
    {
        command.UserId = _userContext.UserId;
        
        ImportTemplate newTemplate = _context.Add(command).Entity;
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetImportTemplate", new { id = newTemplate.Id }, null);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetImportTemplate([FromRoute] int id)
    {
        ImportTemplate template = await _context
            .Templates
            .WithUser(_userContext.UserId)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (template is null) return NotFound();
        
        return Ok(template);
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateImportTemplate([FromRoute] int id, ImportTemplate command)
    {
        if (!await _context.Templates.AnyAsync(t => t.Id == id))
        {
            return NotFound();
        }
        
        _context.Update(command);
        await _context.SaveChangesAsync();

        return Ok();
    }
}