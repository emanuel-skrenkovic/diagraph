using Diagraph.Core.Extensions;
using Diagraph.Infrastructure;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Models;
using Diagraph.Infrastructure.Parsing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase
{
    private readonly DiagraphDbContext _context;
    private readonly IGlucoseDataParser _dataParser;
    private readonly GlucoseDataImport _dataImport;
    private readonly IHashTool _hashTool;

    public DataController
    (
        DiagraphDbContext context, 
        IGlucoseDataParser dataParser,
        GlucoseDataImport dataImport,
        IHashTool hashTool
    )
    {
        _context    = context;
        _dataParser = dataParser;
        _dataImport = dataImport;
        _hashTool   = hashTool;
    }

    [HttpGet]
    public async Task<IActionResult> GetData([FromQuery] DateTime from, [FromQuery] DateTime to)
        => Ok
        (
            await _context
                .GlucoseMeasurements
                .Where(m => m.TakenAt >= from && m.TakenAt < to&& m.Level > 0)
                .OrderBy(m => m.TakenAt)
                .ToListAsync()
        );

    
    [HttpPost]
    public async Task<IActionResult> ImportData(IFormFile file)
    {
        if (file == null) return BadRequest("TODO file null reason");

        string data = await file.ReadAsync();
        string hash = _hashTool.ComputeHash(data);

        if (await _context.Imports.AnyAsync(i => i.Hash == hash)) return Ok();

        IEnumerable<GlucoseMeasurement> measurementData = _dataParser.Parse(data);
        
        Import import = await _dataImport.CreateAsync(measurementData);
        if (import == null) return Ok(); // No data to import
        
        import.Hash = _hashTool.ComputeHash(data);
        
        _context.Imports.Add(import);
        
        await _context.SaveChangesAsync();

        return StatusCode(201);
    }
}