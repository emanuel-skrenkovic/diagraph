namespace Diagraph.Modules.Events.DataImports.Contracts;

public interface IEventTemplateDataParser
{
    IEnumerable<Event> Parse<T>(string data, T template);
}