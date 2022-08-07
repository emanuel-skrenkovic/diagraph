using AutoMapper;
using Diagraph.Modules.Events.Api.DataExports.ExportTemplates.Contracts;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataExports;
using FastEndpoints;

namespace Diagraph.Modules.Events.Api.DataExports.ExportTemplates;

public class CreateExportTemplateEndpoint : Endpoint<CreateExportTemplateCommand>
{
    private readonly IMapper         _mapper;
    private readonly EventsDbContext _dbContext;

    public CreateExportTemplateEndpoint(IMapper mapper, EventsDbContext dbContext)
    {
        _mapper    = mapper;
        _dbContext = dbContext;
    }
    
    public override void Configure() => Post("events/export-templates");

    public override async Task HandleAsync(CreateExportTemplateCommand req, CancellationToken ct)
    {
        var template               = _mapper.Map<ExportTemplate>(req);
        ExportTemplate newTemplate = _dbContext.Add(template).Entity;
        await _dbContext.SaveChangesAsync(ct);

        await SendCreatedAtAsync
        (
            GetExportTemplateEndpoint.Name,
            routeValues:  new { id = newTemplate.Id },
            responseBody: null,
            cancellation: ct
        );
    }
}