using Diagraph.Infrastructure.Api.Extensions;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Modules.GlucoseData.Database;
using Diagraph.Modules.GlucoseData.Imports;
using Diagraph.Modules.GlucoseData.Imports.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.GlucoseData.Api.Imports;

[Authorize]
[ApiController]
[Route("data/[controller]")]
public class ImportsController : ControllerBase
{
    private readonly IUserContext         _userContext;
    private readonly GlucoseDataDbContext _context;
    private readonly IGlucoseDataParser   _dataParser;
    private readonly GlucoseDataImport    _dataImport;
    private readonly IHashTool            _hashTool;
    
    public ImportsController
    (
        IUserContext         userContext,
        GlucoseDataDbContext context, 
        IGlucoseDataParser   dataParser,
        GlucoseDataImport    dataImport,
        IHashTool            hashTool
    )
    {
        _userContext = userContext;
        _context     = context;
        _dataParser  = dataParser;
        _dataImport  = dataImport;
        _hashTool    = hashTool;
    }
    
    [HttpPost]
    public async Task<IActionResult> ImportData(IFormFile file)
    {
        if (file == null) return BadRequest("TODO file null reason");

        // TODO: think about moving this to separate class
        string data = await file.ReadAsync();
        string hash = _hashTool.ComputeHash(data);

        if (await _context.Imports.AnyAsync(i => i.Hash == hash)) return Ok();

        Import import = await _dataImport.CreateAsync(_dataParser.Parse(data));
        if (import == null) return Ok(); // No data to import
        
        import.Hash = _hashTool.ComputeHash(data);
        import.WithUser(_userContext.UserId);
        
        _context.Imports.Add(import);
        await _context.SaveChangesAsync();

        return StatusCode(201);
    }
}