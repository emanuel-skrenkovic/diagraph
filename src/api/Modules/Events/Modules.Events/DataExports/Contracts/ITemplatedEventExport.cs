namespace Diagraph.Modules.Events.DataExports.Contracts;

public interface ITemplatedEventExport
{
    Task<byte[]> ExportEventsAsync(ExportTemplate template, IEnumerable<Event> events);
}