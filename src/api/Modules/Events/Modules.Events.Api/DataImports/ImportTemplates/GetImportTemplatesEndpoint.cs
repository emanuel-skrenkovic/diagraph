using AutoMapper;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports.Csv;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Events.Api.DataImports.ImportTemplates;

public class GetImportTemplatesEndpoint : Endpoint<List<ImportTemplateView>>
{
    private readonly IMapper         _mapper;
    private readonly EventsDbContext _context;
    private readonly IUserContext    _userContext;
    
    public GetImportTemplatesEndpoint(IMapper mapper, EventsDbContext context, IUserContext userContext)
    {
        _mapper      = mapper;
        _context     = context;
        _userContext = userContext;
    }
    
    public override void Configure() => Get("events/import-templates");

    public override async Task HandleAsync(List<ImportTemplateView> req, CancellationToken ct)
    {
        await SendOkAsync
        (
            await _context
                .Templates
                .WithUser(_userContext.UserId)
                .Select(t => ImportTemplateView.FromTemplate<CsvTemplate>(t, _mapper))
                .ToListAsync(ct),
            ct
        ); 
    }
}