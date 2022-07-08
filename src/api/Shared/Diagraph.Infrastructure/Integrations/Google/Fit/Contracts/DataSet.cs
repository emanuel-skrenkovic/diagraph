namespace Diagraph.Infrastructure.Integrations.Google.Fit.Contracts;

public class Dataset
{
    public string DataSourceId { get; set; }

    public string MaxEndTimeNs { get; set; }

    public virtual string MinStartTimeNs { get; set; }

    public virtual string NextPageToken { get; set; }

    public virtual IList<DataPoint> Point { get; set; }

    public virtual string ETag { get; set; } 
}