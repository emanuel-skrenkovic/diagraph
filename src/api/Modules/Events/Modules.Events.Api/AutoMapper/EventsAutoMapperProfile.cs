using AutoMapper;
using Diagraph.Modules.Events.Api.Contracts;
using Diagraph.Modules.Events.Api.DataExports.ExportTemplates.Contracts;
using Diagraph.Modules.Events.Api.DataImports.ImportTemplates.Contracts;
using Diagraph.Modules.Events.Api.Events.Contracts;
using Diagraph.Modules.Events.DataExports;
using Diagraph.Modules.Events.DataImports;

namespace Diagraph.Modules.Events.Api.AutoMapper;

public class EventsAutoMapperProfile : Profile
{
    public EventsAutoMapperProfile()
    {
        CreateMap<Event, EventView>();
    
        CreateMap<EventCreateDto, Event>();
        CreateMap<UpdateEventCommand, Event>();

        CreateMap<EventTagDto, EventTag>().ReverseMap();

        CreateMap<CreateImportTemplateCommand, ImportTemplate>();
        CreateMap<UpdateImportTemplateCommand, ImportTemplate>();
        
        CreateMap<ImportTemplate, ImportTemplateView>();
        
        CreateMap<CreateExportTemplateCommand, ExportTemplate>();
        CreateMap<UpdateExportTemplateCommand, ExportTemplate>();

        CreateMap<ExportTemplate, ExportTemplateView>();
    }
}