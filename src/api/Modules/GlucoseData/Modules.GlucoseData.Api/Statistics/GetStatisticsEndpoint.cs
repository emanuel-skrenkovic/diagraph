using AutoMapper;
using Diagraph.Modules.GlucoseData.Api.Statistics.Contracts;
using Diagraph.Modules.GlucoseData.Database;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.GlucoseData.Api.Statistics;

public class GetStatisticsEndpoint : EndpointWithoutRequest<GlucoseStatisticsView>
{
    private readonly IMapper                   _mapper;
    private readonly DbSet<GlucoseMeasurement> _measurements;

    public GetStatisticsEndpoint
    (
        IMapper              mapper,
        GlucoseDataDbContext context
    )
    {
        _mapper       = mapper;
        _measurements = context.GlucoseMeasurements;
    }
    
    public override void Configure() => Get("data/stats");

    public override async Task HandleAsync(CancellationToken ct)
    {
        var from = Query<DateTime>("from");
        var to   = Query<DateTime>("to");
        
        List<GlucoseMeasurement> measurements = await _measurements
            .Where(m => m.TakenAt >= from && m.TakenAt < to && m.Level > 0)
            .OrderBy(m => m.TakenAt)
            .ToListAsync(ct);
        
        GlucoseStatistics statistics = GlucoseStatistics.Calculate(measurements);
        await SendOkAsync(_mapper.Map<GlucoseStatisticsView>(statistics), ct);
    }
}