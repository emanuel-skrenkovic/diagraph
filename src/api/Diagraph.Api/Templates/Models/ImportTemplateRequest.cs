namespace Diagraph.Api.Templates.Models;

public class ImportTemplateRequest
{
    public string Name { get; set; }
    
    public dynamic Template { get; set; }
}