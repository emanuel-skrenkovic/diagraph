using System.ComponentModel.DataAnnotations;

namespace Diagraph.Modules.Events.Api.DataImports.Commands;

public class CreateImportTemplateCommand
{
     [Required]
     public string Name { get; set; }
     
     [Required]
     public dynamic Data { get; set; }
}