using AutoMapper;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.Events.Api.DataImports.Commands;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports;
using Diagraph.Modules.Events.DataImports.Csv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.DataImports;

[Authorize]
[ApiController]
[Route("events/import-templates")]
public class ImportTemplatesController : ControllerBase
{
    private readonly IMapper         _mapper;
    private readonly EventsDbContext _context;
    private readonly IUserContext    _userContext;
 
    public ImportTemplatesController(IMapper mapper, EventsDbContext context, IUserContext userContext)
    {
        _mapper      = mapper;
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
                .Select(t => ImportTemplateView.FromTemplate<CsvTemplate>(t, _mapper))
                .ToListAsync()
        );

    [HttpPost]
    public async Task<IActionResult> CreateImportTemplate(CreateImportTemplateCommand command)
    {
        ImportTemplate template = _mapper.Map<ImportTemplate>(command);
        template.UserId = _userContext.UserId;
        
        ImportTemplate newTemplate = _context.Add(template).Entity;
        await _context.SaveChangesAsync();

        return CreatedAtAction
        (
            "GetImportTemplate",
            new { id = newTemplate.Id }, 
            null
        );
    }

    [HttpGet]
    [Route("{id}", Name="GetImportTemplate")]
    public async Task<IActionResult> GetImportTemplate([FromRoute] int id)
    {
        ImportTemplate template = await _context
            .Templates
            .WithUser(_userContext.UserId)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (template is null) return NotFound();
        
        return Ok
        (
            ImportTemplateView.FromTemplate<CsvTemplate>(template, _mapper)
        );
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateImportTemplate
    (
        [FromRoute] int id, 
        [FromBody] UpdateImportTemplateCommand command
    )
    {
        ImportTemplate template = await _context.FindAsync<ImportTemplate>(id);
        if (template is null) return NotFound();

        _mapper.Map(command, template);
            
        _context.Update(template);
        await _context.SaveChangesAsync();

        return Ok();
    }
}