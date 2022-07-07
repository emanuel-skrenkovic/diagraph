using Diagraph.Infrastructure.Api.Extensions;
using Diagraph.Infrastructure.Dynamic.Extensions;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports;
using Diagraph.Modules.Events.DataImports.Contracts;
using Diagraph.Modules.Events.DataImports.Csv;
using Diagraph.Modules.Events.DataImports.Extensions;
using Diagraph.Modules.Events.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Diagraph.Modules.Events.Api.DataImports.ImportEvents;

public class ImportEventsEndpoint : EndpointWithoutRequest
{
    private readonly EventsDbContext          _dbContext;
    private readonly IHashTool                _hashTool;
    private readonly IEventTemplateDataParser _dataParser;
    private readonly EventImport              _eventImport;

    public ImportEventsEndpoint
    (
        EventsDbContext          dbContext, 
        IHashTool                hashTool,
        IEventTemplateDataParser dataParser,
        EventImport              eventImport
    )
    {
        _dbContext   = dbContext;
        _hashTool    = hashTool;
        _dataParser  = dataParser;
        _eventImport = eventImport;
    }
    
    public override void Configure() => Post("events/data-import");

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

        ImportTemplate template = await _dbContext
            .Templates
            .FirstOrDefaultAsync(t => t.Name == templateName, ct);

        if (template is null)
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        List<Event> events = _dataParser
            .Parse(await file.ReadAsync(), template.GetData<CsvTemplate>())
            .Select
            (
                e =>
                {
                    e.Source = template.SourceName();
                    return e.WithDiscriminator(_hashTool);
                }
            ).ToList();

        if (!events.Any())
        {
            await SendOkAsync(ct);
            return;
        }

        await _eventImport.ExecuteAsync(events, ct);
        await SendOkAsync(ct);
    }
}