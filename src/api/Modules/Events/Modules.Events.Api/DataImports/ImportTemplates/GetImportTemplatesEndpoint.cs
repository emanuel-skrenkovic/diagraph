using AutoMapper;
using Diagraph.Modules.Events.Api.DataImports.ImportTemplates.Contracts;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports;
using Diagraph.Modules.Events.DataImports.Csv;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.DataImports.ImportTemplates;

public class GetImportTemplatesEndpoint : Endpoint<List<ImportTemplateView>>
{
    private readonly IMapper         _mapper;
    private readonly DbSet<ImportTemplate> _templates;
    
    public GetImportTemplatesEndpoint(IMapper mapper, EventsDbContext dbContext)
    {
        _mapper    = mapper;
        _templates = dbContext.Templates;
    }
    
    public override void Configure() => Get("events/import-templates");

    public override async Task HandleAsync(List<ImportTemplateView> req, CancellationToken ct)
        => await SendOkAsync
        (
            await _templates
                .Select(t => ImportTemplateView.FromTemplate<CsvTemplate>(t, _mapper))
                .ToListAsync(ct),
            ct
        ); 
}