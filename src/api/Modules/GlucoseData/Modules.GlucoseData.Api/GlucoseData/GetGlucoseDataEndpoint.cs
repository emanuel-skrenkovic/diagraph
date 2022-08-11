using AutoMapper;
using Diagraph.Modules.GlucoseData.Api.GlucoseData.Contracts;
using Diagraph.Modules.GlucoseData.Database;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.GlucoseData.Api.GlucoseData;

public class GetGlucoseDataEndpoint : EndpointWithoutRequest<List<GlucoseMeasurementView>>
{
    private readonly IMapper                   _mapper;
    private readonly DbSet<GlucoseMeasurement> _measurements;

    public GetGlucoseDataEndpoint(IMapper mapper, GlucoseDataDbContext context)
    {
        _mapper       = mapper;
        _measurements = context.GlucoseMeasurements;
    }
    
    public override void Configure() => Get("data");

    public override async Task HandleAsync(CancellationToken ct)
    {
        var from = Query<DateTime>("from");
        var to   = Query<DateTime>("to");

        List<GlucoseMeasurement> measurements = await _measurements
            .Where(m => m.TakenAt >= from && m.TakenAt < to && m.Level > 0)
            .OrderBy(static m => m.TakenAt)
            .ToListAsync(ct);
        
        await SendOkAsync(_mapper.Map<List<GlucoseMeasurementView>>(measurements), ct);
    }
}