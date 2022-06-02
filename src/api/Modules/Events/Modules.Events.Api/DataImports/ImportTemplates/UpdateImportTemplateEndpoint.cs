using AutoMapper;
using Diagraph.Modules.Events.Api.DataImports.ImportTemplates.Commands;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports;
using FastEndpoints;

namespace Diagraph.Modules.Events.Api.DataImports.ImportTemplates;

public class UpdateImportTemplateEndpoint : Endpoint<UpdateImportTemplateCommand>
{
    private readonly IMapper         _mapper;
    private readonly EventsDbContext _context;
 
    public UpdateImportTemplateEndpoint(IMapper mapper, EventsDbContext context)
    {
        _mapper  = mapper;
        _context = context;
    }

    public override void Configure()
    {
        Put("events/import-templates/{id}");
    }

    public override async Task HandleAsync(UpdateImportTemplateCommand req, CancellationToken ct)
    {
        int id = Route<int>("id", isRequired: true);
        
        ImportTemplate template = await _context.FindAsync<ImportTemplate>(id);
        if (template is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        _mapper.Map(req, template);
            
        _context.Update(template);
        await _context.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}