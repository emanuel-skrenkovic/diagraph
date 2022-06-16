using AutoMapper;
using Diagraph.Modules.GlucoseData.Api.GlucoseData.Contracts;
using Diagraph.Modules.GlucoseData.Api.Statistics.Contracts;

namespace Diagraph.Modules.GlucoseData.Api.AutoMapper;

public class GlucoseDataAutoMapperProfile : Profile
{
    public GlucoseDataAutoMapperProfile()
    {
        CreateMap<GlucoseMeasurement, GlucoseMeasurementView>();
        CreateMap<GlucoseStatistics, GlucoseStatisticsView>();
    }
}