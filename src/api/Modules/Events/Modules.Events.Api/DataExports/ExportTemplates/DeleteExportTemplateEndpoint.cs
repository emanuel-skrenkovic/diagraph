using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataExports;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.DataExports.ExportTemplates;

public class DeleteExportTemplateEndpoint : EndpointWithoutRequest
{
    private readonly EventsDbContext _dbContext;

    public DeleteExportTemplateEndpoint(EventsDbContext dbContext) => _dbContext = dbContext;

    public override void Configure() => Delete("events/export-templates/{id}");

    public override async Task HandleAsync(CancellationToken ct)
    {
        int id = Route<int>("id");
        
        ExportTemplate template = await _dbContext
            .ExportTemplates
            .SingleOrDefaultAsync(t => t.Id == id, ct);

        if (template is null)
        {
            await SendOkAsync(ct);
            return;
        }

        _dbContext.Remove(template);
        await _dbContext.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}