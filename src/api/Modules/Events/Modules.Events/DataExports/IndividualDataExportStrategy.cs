namespace Diagraph.Modules.Events.DataExports;

public class IndividualDataExportStrategy : IDataExportStrategy
{
    private readonly IDataWriter _writer;

    public IndividualDataExportStrategy(IDataWriter writer) => _writer = writer;
    
    public Task<byte[]> ExportAsync(IEnumerable<Event> events)
        => _writer.WriteEventAsync(events);

    public Task<Stream> ExportStreamAsync(IEnumerable<Event> events)
        => _writer.WriteEventStreamAsync(events);
}