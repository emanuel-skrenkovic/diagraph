using AutoMapper;
using Diagraph.Infrastructure.Dynamic.Extensions;
using Diagraph.Modules.Events.Api.DataExports.ExportTemplates.Contracts;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataExports;
using Diagraph.Modules.Events.DataExports.Csv;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.DataExports.ExportTemplates;

public class GetExportTemplateEndpoint : EndpointWithoutRequest
{
    public const string Name = "GetExportTemplate";

    private readonly IMapper               _mapper;
    private readonly DbSet<ExportTemplate> _templates;

    public GetExportTemplateEndpoint(IMapper mapper, EventsDbContext dbContext)
    {
        _mapper    = mapper;
        _templates = dbContext.ExportTemplates;
    }
    
    public override void Configure()
    {
        Get("events/export-templates/{id}");
        Description(e => e.WithName(Name));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
         int id = Route<int>("id");
         ExportTemplate template = await _templates.FirstOrDefaultAsync(t => t.Id == id, ct);
         
         if (template is null)
         {
             await SendNotFoundAsync(ct);
             return;
         }
         
         var view = _mapper.Map<ExportTemplateView>(template);
         view.Data = template.GetData<CsvExportTemplate>();
         
         await SendOkAsync(view, ct);       
    }
}