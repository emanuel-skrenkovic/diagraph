using AutoMapper;

namespace Diagraph.Modules.Events.DataImports.Csv.AutoMapper;

public class EventsCsvImportAutoMapperProfile : Profile
{
    public EventsCsvImportAutoMapperProfile()
    {
        CreateMap<Dictionary<string, object>, Event>()
            .ForMember
            (
                e => e.OccurredAtUtc,
                conf => conf.MapFrom(dict => GetOrDefault(dict, nameof(Event.OccurredAtUtc)))
            );
    }
    
    private DateTime GetOrDefault(Dictionary<string, object> dict, string key)
    {
        Dictionary<string, object> caseInsensitive = new(dict, StringComparer.OrdinalIgnoreCase);
        if (caseInsensitive.TryGetValue(key, out object occurredAtUtcString))
        {
            string occurredAtUtc = (string) occurredAtUtcString;
            if (string.IsNullOrWhiteSpace(occurredAtUtc)) return default;
            
            return DateTime.SpecifyKind(DateTime.Parse(occurredAtUtc), DateTimeKind.Utc);
        }
        
        return default;
    }
}