using Diagraph.Modules.Events.DataImports.Templates;

namespace Diagraph.Modules.Events.DataImports.Csv;

public class HeaderMappings
{
    public string Header { get; set; }
    
    public IEnumerable<Rule> Rules { get; set; }
    
    public IEnumerable<string> Tags { get; set; }
}