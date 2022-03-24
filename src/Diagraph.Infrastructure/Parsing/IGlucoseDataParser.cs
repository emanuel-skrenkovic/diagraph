using Diagraph.Infrastructure.Models;

namespace Diagraph.Infrastructure.Parsing;

public interface IGlucoseDataParser
{
    IEnumerable<GlucoseMeasurement> Parse(string data);
}