using AutoMapper;
using Diagraph.Modules.Events.Api.DataExports.ExportTemplates.Contracts;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataExports;
using Diagraph.Modules.Events.DataExports.Csv;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.DataExports.ExportTemplates;

public class GetExportTemplatesEndpoint : EndpointWithoutRequest
{
    private readonly IMapper               _mapper;
    private readonly DbSet<ExportTemplate> _exportTemplates;

    public GetExportTemplatesEndpoint(IMapper mapper, EventsDbContext context)
    {
        _mapper          = mapper;
        _exportTemplates = context.ExportTemplates;
    }
    
    public override void Configure() => Get("events/export-templates");

    public override async Task HandleAsync(CancellationToken ct)
        => await SendOkAsync
        (
            await _exportTemplates
                .Select(t => ExportTemplateView.IntoView<CsvExportTemplate>(t, _mapper))
                .ToListAsync(ct),
            ct
        );
}