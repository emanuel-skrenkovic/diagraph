using Diagraph.Modules.Events.DataExports.Contracts;

namespace Diagraph.Modules.Events.DataExports;

public class IndividualDataExportStrategy : IDataExportStrategy
{
    public IEnumerable<Event> Run(IEnumerable<Event> events) => events;
}