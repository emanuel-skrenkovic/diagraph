using Google.Apis.Fitness.v1.Data;

namespace Diagraph.Infrastructure.Integrations.Google.Fit.Contracts;

public class DataPoint
{
    public string DataTypeName { get; set; }

    public string EndTimeNanos { get; set; }

    public string ModifiedTimeMillis { get; set; }

    public string OriginDataSourceId { get; set; }

    public string StartTimeNanos { get; set; }

    public IList<Value> Value { get; set; }

    public virtual string ETag { get; set; } 
}