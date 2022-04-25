using Diagraph.Infrastructure.Models;

namespace Diagraph.Infrastructure.Parsing.Templates;

public interface ITemplateRunner
{
    IEnumerable<Event> MapRow(dynamic row);
}