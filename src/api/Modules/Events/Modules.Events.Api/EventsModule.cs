using Diagraph.Infrastructure.Api;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Events.Contracts;
using Diagraph.Infrastructure.Events.EventStore;
using Diagraph.Infrastructure.EventSourcing;
using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Infrastructure.Integrations.Extensions;
using Diagraph.Infrastructure.Integrations.Google.Fit;
using Diagraph.Infrastructure.Modules;
using Diagraph.Infrastructure.Modules.Extensions;
using Diagraph.Modules.Events.Api.AutoMapper;
using Diagraph.Modules.Events.Database;
using Diagraph.Modules.Events.DataExports;
using Diagraph.Modules.Events.DataImports.Contracts;
using Diagraph.Modules.Events.DataImports.Csv;
using Diagraph.Modules.Events.DataImports.Csv.AutoMapper;
using Diagraph.Modules.Events.DataImports.Templates;
using Diagraph.Modules.Events.DataRemoval;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diagraph.Modules.Events.Api;

public class EventsModule : Module
{
    public override string ModuleName => "events";
    
    protected override void RegisterServices
    (
        IConfiguration configuration, 
        IServiceCollection services
    )
    {
        services.AddAutoMapper(typeof(EventsAutoMapperProfile));
        services.AddAutoMapper(typeof(EventsCsvImportAutoMapperProfile));
        
        services.AddHttpContextAccessor();
        services.AddPostgres<EventsDbContext>
        (
            configuration
                .GetSection(DatabaseConfiguration.SectionName)
                .Get<DatabaseConfiguration>()
        );
        services.AddGoogleIntegration(configuration);

        services.AddScoped<TemplateRunnerFactory>();
        services.AddScoped<IEventTemplateDataParser, EventCsvTemplateDataParser>();

        services.AddScoped<IDataExportStrategy, MergingDataExportStrategy>();
        services.AddScoped<IDataExportStrategy, IndividualDataExportStrategy>();
        services.AddScoped<ExportStrategyContext>();
        services.AddScoped<IDataWriter, CsvDataWriter>();
        
        services.AddScoped<EventImport>();

        services.AddScoped<IGoogleFit, GoogleFit>();

        services.AddScoped<IEventDispatcher, EventStoreEventDispatcher>();
        services.AddScoped<ICorrelationContext, CorrelationContext>();
        
        services.AddTransient<EventSubscriber>();
        services.AddScoped<IEventSubscription, EventDataRemovalListener>();
    }
}