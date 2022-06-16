using AutoMapper;
using Diagraph.Modules.Events.DataImports;

namespace Diagraph.Modules.Events.Api.DataImports.ImportTemplates.Contracts;

public class ImportTemplateView
{
    public int Id { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime UpdatedAtUtc { get; set; }
    
    public string Name { get; set; }
    
    public dynamic Data { get; set; }

    public static ImportTemplateView FromTemplate<T>(ImportTemplate template, IMapper mapper)
    {
         ImportTemplateView view = mapper.Map<ImportTemplateView>(template);
         view.Data = template.Get<T>();

         return view;
    }
}