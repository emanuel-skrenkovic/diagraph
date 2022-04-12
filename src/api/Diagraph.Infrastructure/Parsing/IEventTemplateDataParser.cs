using Diagraph.Infrastructure.Models;

namespace Diagraph.Infrastructure.Parsing;

public interface IEventTemplateDataParser<T>
{
    IEnumerable<Event> Parse(string data, T template);
}