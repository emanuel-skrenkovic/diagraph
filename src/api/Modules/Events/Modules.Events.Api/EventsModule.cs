using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Modules;
using Diagraph.Infrastructure.Modules.Extensions;
using Diagraph.Modules.Events.Api.AutoMapper;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataImports.Contracts;
using Diagraph.Modules.Events.DataImports.Csv;
using Diagraph.Modules.Events.DataImports.Templates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diagraph.Modules.Events.Api;

public class EventsModule : Module
{
    public override string ModuleName => "events";
    
    protected override void RegisterServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddAutoMapper(typeof(EventsAutoMapperProfile));
        
        services.AddPostgres<EventsDbContext>
        (
            configuration
                .GetSection(DatabaseConfiguration.SectionName)
                .Get<DatabaseConfiguration>()
        );

        services.AddScoped<TemplateRunnerFactory>();
        services.AddScoped<IEventTemplateDataParser, EventCsvTemplateDataParser>();
    }
}