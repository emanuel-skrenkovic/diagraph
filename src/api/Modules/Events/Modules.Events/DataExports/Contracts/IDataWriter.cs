namespace Diagraph.Modules.Events.DataExports.Contracts;

public interface IDataWriter
{
    Task<byte[]> WriteEventAsync(IEnumerable<Event> events);
    
    Task<Stream> WriteEventStreamAsync(IEnumerable<Event> events);
}