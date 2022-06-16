using Microsoft.AspNetCore.Http;

namespace Diagraph.Modules.Events.Api.DataImports.ImportEvents.Contracts;

public class ImportEventsCommand
{
    public IFormFile File { get; set; }
    
    public string TemplateName { get; set; }
}