using AutoMapper;
using Diagraph.Infrastructure.Auth;
using Diagraph.Modules.Events.Api.DataImports.ImportTemplates.Commands;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports;
using FastEndpoints;

namespace Diagraph.Modules.Events.Api.DataImports.ImportTemplates;

public class CreateImportTemplateEndpoint : Endpoint<CreateImportTemplateCommand>
{
    private readonly IMapper         _mapper;
    private readonly EventsDbContext _context;
    private readonly IUserContext    _userContext;
 
    public CreateImportTemplateEndpoint(IMapper mapper, EventsDbContext context, IUserContext userContext)
    {
        _mapper      = mapper;
        _context     = context;
        _userContext = userContext;
    }
    
    public override void Configure() => Post("events/import-templates");

    public override async Task HandleAsync(CreateImportTemplateCommand req, CancellationToken ct)
    {
        ImportTemplate template = _mapper.Map<ImportTemplate>(req);
        template.UserId = _userContext.UserId;
        
        ImportTemplate newTemplate = _context.Add(template).Entity;
        await _context.SaveChangesAsync(ct);

        await SendCreatedAtAsync
        (
            GetImportTemplateEndpoint.Name,
            routeValues:  new { id = newTemplate.Id },
            responseBody: null,
            cancellation: ct
        );
    }
}