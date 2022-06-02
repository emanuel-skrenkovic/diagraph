using AutoMapper;
using Diagraph.Modules.Events.Api.DataImports.ImportTemplates;
using Diagraph.Modules.Events.Api.DataImports.ImportTemplates.Commands;
using Diagraph.Modules.Events.Api.Events.Commands;
using Diagraph.Modules.Events.DataImports;

namespace Diagraph.Modules.Events.Api.AutoMapper;

public class EventsAutoMapperProfile : Profile
{
    public EventsAutoMapperProfile()
    {
        CreateMap<EventView, Event>();
    
        CreateMap<CreateEventCommand, Event>();
        CreateMap<UpdateEventCommand, Event>();

        CreateMap<CreateImportTemplateCommand, ImportTemplate>();
        CreateMap<UpdateImportTemplateCommand, ImportTemplate>();

        CreateMap<ImportTemplate, ImportTemplateView>();
    }
}