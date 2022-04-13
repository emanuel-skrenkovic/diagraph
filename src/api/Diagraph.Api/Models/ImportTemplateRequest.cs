namespace Diagraph.Api.Models;

public class ImportTemplateRequest
{
    public string Name { get; set; }
    
    public dynamic Template { get; set; }
}