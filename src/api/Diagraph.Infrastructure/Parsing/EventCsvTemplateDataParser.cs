using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Diagraph.Infrastructure.Models;
using Diagraph.Infrastructure.Parsing.Templates;

namespace Diagraph.Infrastructure.Parsing;

public class EventCsvTemplateDataParser : IEventTemplateDataParser
{
    private static readonly CsvConfiguration Configuration = new(CultureInfo.InvariantCulture)
    {
        Delimiter       = ",",
        HasHeaderRecord = true
    };

    private readonly TemplateRunnerFactory _runnerFactory;

    public EventCsvTemplateDataParser(TemplateRunnerFactory runnerFactory)
        => _runnerFactory = runnerFactory;
    
    public IEnumerable<Event> Parse<T>(string data, T template)
    {
        Ensure.NotNullOrEmpty(data);
        
        MemoryStream dataStream   = new(Encoding.UTF8.GetBytes(data));
        using StreamReader reader = new(dataStream);
        using CsvReader csv       = new(reader, Configuration);

        csv.Read();
        
        List<Event> events = new();
        dynamic row        = csv.GetRecord<dynamic>();

        ITemplateRunner runner = _runnerFactory.FromTemplate(template);
        while (row is not null)
        { 
            foreach (Event @event in runner.MapRow(row))
            {
                events.Add(@event);
            }
            
            /*
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
            */

            row = csv.GetRecord<dynamic>(); 
        }

        return events;
    }
}