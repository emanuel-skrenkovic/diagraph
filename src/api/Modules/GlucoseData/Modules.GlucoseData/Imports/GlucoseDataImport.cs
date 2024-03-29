using Diagraph.Modules.GlucoseData.Database;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.GlucoseData.Imports;

public class GlucoseDataImport
{
    private readonly DbSet<GlucoseMeasurement> _measurements;

    public GlucoseDataImport(GlucoseDataDbContext context)
        => _measurements = context.GlucoseMeasurements;

    public async Task<Import> CreateAsync(IEnumerable<GlucoseMeasurement> measurementData)
    {
        GlucoseMeasurement lastMeasurement = await _measurements
            .OrderByDescending(static m => m.TakenAt)
            .FirstOrDefaultAsync();
        DateTime? lastMeasurementAt = lastMeasurement?.TakenAt;

        List<GlucoseMeasurement> newMeasurementData = lastMeasurement is not null
            ? measurementData.Where(m => m.TakenAt > lastMeasurementAt).ToList()
            : measurementData.ToList();

        if (!newMeasurementData.Any()) return null; // TODO: result?
           
        return new()
        {
            Measurements = newMeasurementData
        };
    }
}