using AutoMapper;
using Diagraph.Modules.Events.Api.DataExports.ExportTemplates.Contracts;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataExports;
using FastEndpoints;

namespace Diagraph.Modules.Events.Api.DataExports.ExportTemplates;

public class UpdateExportTemplateEndpoint : Endpoint<UpdateExportTemplateCommand>
{
    private readonly IMapper         _mapper;
    private readonly EventsDbContext _dbContext;

    public UpdateExportTemplateEndpoint(IMapper mapper, EventsDbContext dbContext)
    {
        _mapper    = mapper;
        _dbContext = dbContext;
    }
    
    public override void Configure() => Put("events/export-templates/{id}");

    public override async Task HandleAsync(UpdateExportTemplateCommand req, CancellationToken ct)
    {
        int id = Route<int>("id", isRequired: true);
        
        var template = await _dbContext.FindAsync<ExportTemplate>(id);
        if (template is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        _mapper.Map(req, template);
            
        _dbContext.Update(template);
        await _dbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}