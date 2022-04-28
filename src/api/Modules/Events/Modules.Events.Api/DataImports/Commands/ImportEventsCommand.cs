using Microsoft.AspNetCore.Http;

namespace Diagraph.Modules.Events.Api.DataImports.Commands;

public class ImportEventsCommand
{
    public IFormFile File { get; set; }
    
    public string TemplateName { get; set; }
}