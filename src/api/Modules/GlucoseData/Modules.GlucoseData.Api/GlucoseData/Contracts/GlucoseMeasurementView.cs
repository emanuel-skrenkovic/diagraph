using Diagraph.Infrastructure.Models;

namespace Diagraph.Modules.GlucoseData.Api.GlucoseData.Contracts;

public class GlucoseMeasurementView
{
    public Guid UserId { get; set; }
    
    public float Level { get; set; }
    
    public DateTime TakenAt { get; set; }

    public GlucoseUnit Unit { get; set; } = GlucoseUnit.MmolPerLitre;
}