using System.Security.Cryptography;
using System.Text;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Models;
using Diagraph.Infrastructure.Parsing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ImportsController : ControllerBase
{
    private readonly DiagraphDbContext _context;
    private readonly IGlucoseDataParser _dataParser;

    public ImportsController(DiagraphDbContext context, IGlucoseDataParser dataParser)
    {
        _context    = context;
        _dataParser = dataParser;
    } 
    
    [HttpPost]
    public async Task<IActionResult> ImportData(IFormFile file)
    {
        if (file == null) return BadRequest("TODO file null reason");

        MemoryStream dataStream = new MemoryStream();
        await file.CopyToAsync(dataStream);

        string data = Encoding.UTF8.GetString(dataStream.ToArray());

        using SHA1 sha1  = SHA1.Create();
        byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(data));
        string hash      = Convert.ToBase64String(hashBytes);

        if (await _context.Imports.AnyAsync(i => i.Hash == hash)) return Ok();

        Import import = new() { Hash = hash };
        import = _dataParser.Parse(import, data);
        
        _context.Imports.Add(import);
        
        await _context.SaveChangesAsync();

        return StatusCode(201);
    }
}