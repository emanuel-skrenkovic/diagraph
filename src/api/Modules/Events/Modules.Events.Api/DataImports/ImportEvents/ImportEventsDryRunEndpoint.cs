using Diagraph.Infrastructure.Api.Extensions;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports;
using Diagraph.Modules.Events.DataImports.Contracts;
using Diagraph.Modules.Events.DataImports.Csv;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Diagraph.Modules.Events.Api.DataImports.ImportEvents;

public class ImportEventsDryRunEndpoint : EndpointWithoutRequest
{
    private readonly EventsDbContext          _context;
    private readonly IUserContext             _userContext;
    private readonly IEventTemplateDataParser _dataParser;

    public ImportEventsDryRunEndpoint
    (
        EventsDbContext          context, 
        IUserContext             userContext,
        IEventTemplateDataParser dataParser
    )
    {
        _context     = context;
        _userContext = userContext;
        _dataParser  = dataParser;
    }
    
    public override void Configure() => Post("events/data-import/dry-run");

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!Form.TryGetValue("templateName", out StringValues templateNameParam))
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        string templateName = templateNameParam;

        IFormFile file = Files.GetFile("file");
        if (file is null)
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }
        
        ImportTemplate template = await _context
            .Templates
            .WithUser(_userContext.UserId)
            .FirstOrDefaultAsync(t => t.Name == templateName, ct);

        if (template is null)
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }
        
        await SendOkAsync
        (
            _dataParser.Parse
            (
                await file.ReadAsync(), 
                template.Get<CsvTemplate>()
            ), 
            ct
        );
    }
}