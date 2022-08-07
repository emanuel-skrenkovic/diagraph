namespace Diagraph.Modules.Events.DataExports.Contracts;

public interface IDataExportStrategy
{ 
    // Task<byte[]> ExportAsync(IEnumerable<Event> events);
    //
    // Task<Stream> ExportStreamAsync(IEnumerable<Event> events);

    IEnumerable<Event> Run(IEnumerable<Event> events);
}