using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Diagraph.Infrastructure.Models;
using Diagraph.Infrastructure.Parsing.Templates;

namespace Diagraph.Infrastructure.Parsing;

public class EventCsvTemplateDataParser : IEventTemplateDataParser<CsvTemplate>
{
    private static readonly CsvConfiguration Configuration = new(CultureInfo.InvariantCulture)
    {
        Delimiter       = ",",
        HasHeaderRecord = true
    };
    
    public IEnumerable<Event> Parse(string data, CsvTemplate template)
    {
        Ensure.NotNullOrEmpty(data);
        
        MemoryStream dataStream   = new(Encoding.UTF8.GetBytes(data));
        using StreamReader reader = new(dataStream);
        using CsvReader csv       = new(reader, Configuration);

        csv.Read();
        
        List<Event> events = new();
        dynamic row        = csv.GetRecord<dynamic>();
        
        // TODO: Convert to GetRecords<dynamic>().Select(...).
        while (row is not null)
        {
            var expando = (IDictionary<string, object>)row;

            events = events.Concat
            (
                template.HeaderMappings.Select
                (
                    m => new Event
                    {
                        Text = (string)expando[m.Header],
                        Tags = m
                            .Tags
                            .Select(t => new EventTag { Name = t })
                            .ToList()
                    }
                )
            ).ToList();

            row = csv.GetRecord<dynamic>(); 
        }

        return events;
    }
}