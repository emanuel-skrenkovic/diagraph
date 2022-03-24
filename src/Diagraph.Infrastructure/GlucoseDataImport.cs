using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Infrastructure;

public class GlucoseDataImport
{
    private readonly DiagraphDbContext _context;

    public GlucoseDataImport(DiagraphDbContext context)
        => _context = context;

    public async Task<Import> CreateAsync(IEnumerable<GlucoseMeasurement> measurementData)
    {
        GlucoseMeasurement lastMeasurement = await _context
            .GlucoseMeasurements
            .OrderByDescending(m => m.TakenAt)
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