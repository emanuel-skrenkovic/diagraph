namespace Diagraph.Modules.Events.Api.DataExports.ExportTemplates.Contracts;

public class UpdateExportTemplateCommand
{
    public string Name { get; set; }
    
    public dynamic Data { get; set; } 
}