using Diagraph.Infrastructure.Models;

namespace Diagraph.Infrastructure.Parsing;

public interface IEventTemplateDataParser
{
    IEnumerable<Event> Parse<T>(string data, T template);
}