using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Infrastructure.Modules;
using Diagraph.Infrastructure.Modules.Extensions;
using Diagraph.Modules.GlucoseData.Api.AutoMapper;
using Diagraph.Modules.GlucoseData.Database;
using Diagraph.Modules.GlucoseData.Imports;
using Diagraph.Modules.GlucoseData.Imports.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diagraph.Modules.GlucoseData.Api;

public class GlucoseDataModule : Module
{
    public override string ModuleName => "glucose-data";
    protected override void RegisterServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddAutoMapper(typeof(GlucoseDataAutoMapperProfile));
        services.AddPostgres<GlucoseDataDbContext>
        (
            // TODO: why not make pulling configuration also automatic?
            configuration
                .GetSection(DatabaseConfiguration.SectionName)
                .Get<DatabaseConfiguration>()
        );
        
        services.AddScoped<IGlucoseDataParser, LibreViewCsvGlucoseDataParser>();
        services.AddScoped<GlucoseDataImport>();
        services.AddScoped<IHashTool, Sha1HashTool>();
    }
}