namespace Diagraph.Api.Models;

public class ImportEventsRequest
{
    public IFormFile File { get; set; }
    
    public string TemplateName { get; set; }
}