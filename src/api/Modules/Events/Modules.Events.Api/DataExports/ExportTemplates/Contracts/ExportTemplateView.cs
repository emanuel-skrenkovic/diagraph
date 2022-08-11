using AutoMapper;
using Diagraph.Infrastructure.Dynamic.Extensions;
using Diagraph.Modules.Events.DataExports;

namespace Diagraph.Modules.Events.Api.DataExports.ExportTemplates.Contracts;

public class ExportTemplateView
{
    public int Id { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime UpdatedAtUtc { get; set; }
    
    public string Name { get; set; }
    
    public dynamic Data { get; set; } 
    
    public static ExportTemplateView IntoView<T>(ExportTemplate template, IMapper mapper)
    {
        ExportTemplateView view = mapper.Map<ExportTemplateView>(template);
        view.Data = template.GetData<T>();
        
        return view;
    }
}