namespace Diagraph.Modules.GlucoseData.Imports.Contracts;

public interface IGlucoseDataParser
{
    IEnumerable<GlucoseMeasurement> Parse(string data);
}