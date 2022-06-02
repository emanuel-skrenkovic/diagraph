using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.GlucoseData.Database;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.GlucoseData.Api.GlucoseData;

public class GetGlucoseDataEndpoint : EndpointWithoutRequest<List<GlucoseMeasurement>>
{
    private readonly GlucoseDataDbContext _context;
    private readonly IUserContext         _userContext;

    public GetGlucoseDataEndpoint(GlucoseDataDbContext context, IUserContext userContext)
    {
        _context     = context;
        _userContext = userContext;
    }
    
    public override void Configure() => Get("data");

    public override async Task HandleAsync(CancellationToken ct)
    {
        var from = Query<DateTime>("from");
        var to   = Query<DateTime>("to");
        
        await SendOkAsync
        (
            await _context
                .GlucoseMeasurements
                .WithUser(_userContext.UserId)
                .Where(m => m.TakenAt >= from && m.TakenAt < to && m.Level > 0)
                .OrderBy(m => m.TakenAt)
                .ToListAsync(ct),
            ct
        );
    }
}