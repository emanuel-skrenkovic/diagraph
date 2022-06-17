using AutoMapper;
using Diagraph.Modules.Events.Api.DataImports.ImportTemplates.Contracts;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports;
using Diagraph.Modules.Events.DataImports.Csv;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.DataImports.ImportTemplates;

public class GetImportTemplateEndpoint : EndpointWithoutRequest
{
    public const string Name = "GetImportTemplate";
    
    private readonly IMapper               _mapper;
    private readonly DbSet<ImportTemplate> _templates;
 
    public GetImportTemplateEndpoint(IMapper mapper, EventsDbContext dbContext)
    {
        _mapper    = mapper;
        _templates = dbContext.Templates;
    }
    
    public override void Configure()
    {
        Get("events/import-templates/{id}");
        Description(e => e.WithName(Name));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        int id = Route<int>("id", isRequired: true);
        
        ImportTemplate template = await _templates.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (template is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        await SendOkAsync(ImportTemplateView.FromTemplate<CsvTemplate>(template, _mapper), ct);
    }
}