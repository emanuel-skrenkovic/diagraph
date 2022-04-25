using Diagraph.Api.Models;
using Diagraph.Infrastructure;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Infrastructure.Parsing.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Api.Controllers;

[Authorize]
[ApiController]
[Route("import-templates")]
public class ImportTemplatesController : ControllerBase
{
    private readonly DiagraphDbContext _context;
    private readonly IUserContext      _userContext;
 
    public ImportTemplatesController(DiagraphDbContext context, IUserContext userContext)
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
    public async Task<IActionResult> CreateImportTemplate(ImportTemplateRequest request)
    {
        ImportTemplate template = new()
        {
            UserId = _userContext.UserId,
            Name   = request.Name,
            Data   = request.Template.ToString()
        };

        _context.Add(template);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateImportTemplate(ImportTemplateRequest request)
    {
        ImportTemplate template = await _context
            .Templates
            .WithUser(_userContext.UserId)
            .FirstOrDefaultAsync(t => t.Name == request.Name);

        if (template is null) return NotFound();

        template.Data = request.Template;
        
        _context.Update(template);
        await _context.SaveChangesAsync();

        return Ok();
    }
}