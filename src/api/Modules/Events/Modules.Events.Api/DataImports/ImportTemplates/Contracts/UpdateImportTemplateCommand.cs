namespace Diagraph.Modules.Events.Api.DataImports.ImportTemplates.Contracts;

public class UpdateImportTemplateCommand
{
    public string Name { get; set; }
     
    public dynamic Data { get; set; } 
}