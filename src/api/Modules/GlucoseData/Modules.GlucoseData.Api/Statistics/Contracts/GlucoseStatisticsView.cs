namespace Diagraph.Modules.GlucoseData.Api.Statistics.Contracts;

public class GlucoseStatisticsView
{
    public float Mean { get; init; }

    public float Median { get; init; }

    public float GlucoseManagementIndicator { get; init; }

    public float GlucoseManagementIndicatorPercentage { get; init; }
}