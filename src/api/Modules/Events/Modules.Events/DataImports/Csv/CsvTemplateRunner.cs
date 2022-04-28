using AutoMapper;
using Diagraph.Modules.Events.DataImports.Contracts;
using Diagraph.Modules.Events.DataImports.Templates;

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
        List<Event> events = new();

        var expando = (IDictionary<string, object>)row;
        
        foreach (HeaderMappings map in _template.HeaderMappings)
        {
            Dictionary<string, object> eventData = new()
            {
                [nameof(Event.Text)] = expando[map.Header],
                [nameof(Event.Tags)] = map
                    .Tags
                    .Select(tag => new EventTag { Name = tag })
                    .ToList()
            };

            if (map.Rules?.Any() == true)
            {
                TemplateLanguageParser parser = new(eventData, row);
                foreach (Rule rule in map.Rules)
                {
                    parser.ApplyRule
                    (
                        rule.Expression,
                        new[] { "occurredAtUtc" } // TODO: configuration?
                    );
                }
            }
            
            events.Add(_mapper.Map<Event>(eventData));
        }

        return events;
    } 
}