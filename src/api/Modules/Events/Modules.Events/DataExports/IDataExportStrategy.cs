namespace Diagraph.Modules.Events.DataExports;

public interface IDataExportStrategy
{ 
    Task<byte[]> ExportAsync(IEnumerable<Event> events);
    
    Task<Stream> ExportStreamAsync(IEnumerable<Event> events);
}