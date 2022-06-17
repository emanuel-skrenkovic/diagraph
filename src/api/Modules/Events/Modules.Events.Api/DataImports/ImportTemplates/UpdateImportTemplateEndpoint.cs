using AutoMapper;
using Diagraph.Modules.Events.Api.DataImports.ImportTemplates.Contracts;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports;
using FastEndpoints;

namespace Diagraph.Modules.Events.Api.DataImports.ImportTemplates;

public class UpdateImportTemplateEndpoint : Endpoint<UpdateImportTemplateCommand>
{
    private readonly IMapper         _mapper;
    private readonly EventsDbContext _dbContext;
 
    public UpdateImportTemplateEndpoint(IMapper mapper, EventsDbContext dbContext)
    {
        _mapper  = mapper;
        _dbContext = dbContext;
    }

    public override void Configure() => Put("events/import-templates/{id}");

    public override async Task HandleAsync(UpdateImportTemplateCommand req, CancellationToken ct)
    {
        int id = Route<int>("id", isRequired: true);
        
        ImportTemplate template = await _dbContext.FindAsync<ImportTemplate>(id);
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