using AutoMapper;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
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
    
    private readonly IMapper         _mapper;
    private readonly EventsDbContext _context;
    private readonly IUserContext    _userContext;
 
    public GetImportTemplateEndpoint(IMapper mapper, EventsDbContext context, IUserContext userContext)
    {
        _mapper      = mapper;
        _context     = context;
        _userContext = userContext;
    }
    
    public override void Configure()
    {
        Get("events/import-templates/{id}");
        Description(e => e.WithName(Name));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        int id = Route<int>("id", isRequired: true);
        
        ImportTemplate template = await _context
            .Templates
            .WithUser(_userContext.UserId)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (template is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        await SendOkAsync
        (
            ImportTemplateView.FromTemplate<CsvTemplate>(template, _mapper),
            ct
        );
    }
}