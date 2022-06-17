using AutoMapper;
using Diagraph.Modules.Events.Api.DataImports.ImportTemplates.Contracts;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports;
using FastEndpoints;

namespace Diagraph.Modules.Events.Api.DataImports.ImportTemplates;

public class CreateImportTemplateEndpoint : Endpoint<CreateImportTemplateCommand>
{
    private readonly IMapper         _mapper;
    private readonly EventsDbContext _dbContext;
 
    public CreateImportTemplateEndpoint(IMapper mapper, EventsDbContext dbContext)
    {
        _mapper    = mapper;
        _dbContext = dbContext;
    }
    
    public override void Configure() => Post("events/import-templates");

    public override async Task HandleAsync(CreateImportTemplateCommand req, CancellationToken ct)
    {
        ImportTemplate template    = _mapper.Map<ImportTemplate>(req);
        ImportTemplate newTemplate = _dbContext.Add(template).Entity;
        await _dbContext.SaveChangesAsync(ct);

        await SendCreatedAtAsync
        (
            GetImportTemplateEndpoint.Name,
            routeValues:  new { id = newTemplate.Id },
            responseBody: null,
            cancellation: ct
        );
    }
}