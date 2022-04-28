using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Diagraph.Infrastructure;
using Diagraph.Modules.Events.DataImports.Contracts;
using Diagraph.Modules.Events.DataImports.Templates;

namespace Diagraph.Modules.Events.DataImports.Csv;

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

            row = csv.GetRecord<dynamic>(); 
        }

        return events;
    }
}