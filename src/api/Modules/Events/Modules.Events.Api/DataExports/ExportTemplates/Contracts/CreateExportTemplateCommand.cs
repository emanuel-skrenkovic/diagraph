using System.ComponentModel.DataAnnotations;

namespace Diagraph.Modules.Events.Api.DataExports.ExportTemplates.Contracts;

public class CreateExportTemplateCommand
{
    [Required] public string Name { get; set; }
    
    [Required] public dynamic Data { get; set; }
}