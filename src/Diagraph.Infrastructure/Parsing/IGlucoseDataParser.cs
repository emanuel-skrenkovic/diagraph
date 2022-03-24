using Diagraph.Infrastructure.Models;

namespace Diagraph.Infrastructure.Parsing;

public interface IGlucoseDataParser
{
    Import Parse(Import import, string data);
}