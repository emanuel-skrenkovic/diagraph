namespace Diagraph.Infrastructure.Cache.Redis;

public class RedisConfiguration
{
    public const string SectionName = nameof(RedisConfiguration);
    
    public string ConnectionString { get; set; }
    
    public int? Database { get; set; }
}