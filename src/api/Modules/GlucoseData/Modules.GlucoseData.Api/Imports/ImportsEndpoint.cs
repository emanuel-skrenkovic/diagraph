using Diagraph.Infrastructure.Api.Extensions;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Modules.GlucoseData.Database;
using Diagraph.Modules.GlucoseData.Imports;
using Diagraph.Modules.GlucoseData.Imports.Contracts;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.GlucoseData.Api.Imports;

public class ImportsEndpoint : EndpointWithoutRequest
{
    private readonly IUserContext         _userContext;
    private readonly GlucoseDataDbContext _context;
    private readonly IGlucoseDataParser   _dataParser;
    private readonly GlucoseDataImport    _dataImport;
    private readonly IHashTool            _hashTool;
    
    public ImportsEndpoint
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

    public override void Configure()
    {
        Post("data/imports");
        AllowFileUploads();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        IFormFile file = Files.GetFile("file");

        if (file is null)
        {
            await SendAsync("TODO file null reason", 400, ct);
            return;
        }

        // TODO: think about moving this to separate class
        string data = await file.ReadAsync();
        string hash = _hashTool.ComputeHash(data);

        if (await _context.Imports.AnyAsync(i => i.Hash == hash, ct))
        {
            await SendOkAsync(ct);
            return;
        }

        Import import = await _dataImport.CreateAsync(_dataParser.Parse(data));
        if (import == null) 
        {
             await SendOkAsync(ct);
             return;  // No data to import
        }
        
        import.Hash = _hashTool.ComputeHash(data);
        import.WithUser(_userContext.UserId);
        
        _context.Imports.Add(import);
        await _context.SaveChangesAsync(ct);

        await this.SendCreated(ct);
    }
}