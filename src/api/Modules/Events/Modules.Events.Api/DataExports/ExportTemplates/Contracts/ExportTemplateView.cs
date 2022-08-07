namespace Diagraph.Modules.Events.Api.DataExports.ExportTemplates.Contracts;

public class ExportTemplateView
{
    public int Id { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime UpdatedAtUtc { get; set; }
    
    public string Name { get; set; }
    
    public dynamic Data { get; set; } 
}