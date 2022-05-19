using AutoMapper;
using Diagraph.Modules.Events.DataImports.Contracts;

namespace Diagraph.Modules.Events.DataImports.Csv;

public class CsvTemplateRunner : ITemplateRunner
{
    private readonly IMapper     _mapper;
    private readonly CsvTemplate _template;
    
    public CsvTemplateRunner(IMapper mapper, CsvTemplate template)
    {
        _mapper   = mapper;
        _template = template;
    }

    public IEnumerable<Event> MapRow(dynamic row)
    {
        var expando = (IDictionary<string, object>)row;

        foreach (HeaderMappings map in _template.HeaderMappings)
        {
            Dictionary<string, object> initialData = new()
            {
                [nameof(Event.Text)] = expando[map.Header],
                [nameof(Event.Tags)] = map.Tags
            };

            if (map.Rules?.Any() != true) continue;
            
            foreach (Rule rule in map.Rules)
            {
                TemplateLanguageParser parser = new(initialData, row);
                foreach (var mappingResult in 
                         parser.ApplyRule(rule.Expression, new[] { "occurredAtUtc" }))
                {
                    yield return _mapper.Map<Event>(mappingResult);
                }
            }
        }
    }
}