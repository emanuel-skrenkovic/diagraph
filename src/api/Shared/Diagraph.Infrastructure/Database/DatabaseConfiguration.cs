namespace Diagraph.Infrastructure.Database;

public class DatabaseConfiguration
{
    public const string SectionName = nameof(DatabaseConfiguration);
    
    public string ConnectionString { get; set; }
    
    public string MigrationsAssembly { get; set; }
}