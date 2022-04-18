using System.Globalization;
using System.Text;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Diagraph.Infrastructure.Models;
using Diagraph.Infrastructure.Parsing.Language;
using Diagraph.Infrastructure.Parsing.Templates;

namespace Diagraph.Infrastructure.Parsing;

public class EventCsvTemplateDataParser : IEventTemplateDataParser<CsvTemplate>
{
    private static readonly CsvConfiguration Configuration = new(CultureInfo.InvariantCulture)
    {
        Delimiter       = ",",
        HasHeaderRecord = true
    };

    private readonly IMapper _mapper;
    
    public EventCsvTemplateDataParser(IMapper mapper)
        => _mapper = mapper;
    
    public IEnumerable<Event> Parse(string data, CsvTemplate template)
    {
        Ensure.NotNullOrEmpty(data);
        
        MemoryStream dataStream   = new(Encoding.UTF8.GetBytes(data));
        using StreamReader reader = new(dataStream);
        using CsvReader csv       = new(reader, Configuration);

        csv.Read();
        
        List<Event> events = new();
        dynamic row        = csv.GetRecord<dynamic>();
        
        // TODO: Maybe convert to GetRecords<dynamic>().Select(...).
        while (row is not null)
        {
            var expando = (IDictionary<string, object>)row;
            
            foreach (HeaderMappings map in template.HeaderMappings)
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
                    TemplateLanguageParser parser = new(eventData, expando);
                    foreach (Rule rule in map.Rules)
                    {
                        parser.ApplyRule
                        (
                            rule.Expression, 
                            new [] { "occurredAtUtc" } // TODO: configuration?
                        );
                    }
                }

                events.Add(_mapper.Map<Event>(eventData));
            }

            row = csv.GetRecord<dynamic>(); 
        }

        return events;
    }
}