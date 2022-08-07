namespace Diagraph.Modules.Events.DataExports.Csv;

public class CsvExportTemplate
{
    /// <summary>
    /// Event that contains a tag will be placed into the csv row
    /// with a header that matches the tag name.
    /// </summary>
    public IEnumerable<string> Headers { get; set; }
}