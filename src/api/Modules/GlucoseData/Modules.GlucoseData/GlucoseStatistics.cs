namespace Diagraph.Modules.GlucoseData;

public class GlucoseStatistics
{
    public float Mean { get; init; }
    
    public float Median { get; init; }
    
    public float GlucoseManagementIndicator { get; init; }
    
    public float GlucoseManagementIndicatorPercentage { get; init; }

    public static GlucoseStatistics Calculate(IEnumerable<GlucoseMeasurement> measurements)
    {
        List<float> levels = measurements.Select(m => m.Level).ToList();
        if (!levels.Any()) return new();
        
        float mean = levels.Sum() / levels.Count;
        
        List<float> ordered = levels.OrderBy(l => l).ToList();
        int         middle  = (int) Math.Floor((float) (ordered.Count() / 2));

        float median;
        if (ordered.Count % 2 == 0) median = (ordered[middle - 1] + ordered[middle]) / 2;
        else                        median = ordered[middle];

        return new()
        {
            Mean                                 = mean,
            Median                               = median,
            GlucoseManagementIndicator           = 12.71f + 4.70587f * mean,
            GlucoseManagementIndicatorPercentage = 3.31f + 0.02392f * 18 * mean
        };
    }
}