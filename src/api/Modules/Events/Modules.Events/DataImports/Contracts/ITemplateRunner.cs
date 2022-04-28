namespace Diagraph.Modules.Events.DataImports.Contracts;

public interface ITemplateRunner
{
    IEnumerable<Event> MapRow(dynamic row);
}