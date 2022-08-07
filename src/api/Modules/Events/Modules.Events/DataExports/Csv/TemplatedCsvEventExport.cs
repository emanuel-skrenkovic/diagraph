using System.Globalization;
using CsvHelper;
using Diagraph.Infrastructure.Dynamic.Extensions;
using Diagraph.Modules.Events.DataExports.Contracts;

namespace Diagraph.Modules.Events.DataExports.Csv;

// TODO: need an interface. Might want to export to something other than CSV sometimes.
public class TemplatedCsvEventExport: ITemplatedEventExport
{
    public async Task<byte[]> ExportEventsAsync(ExportTemplate template, IEnumerable<Event> events)
    {
        IEnumerable<IGrouping<DateTime, Event>> groupedEvents = events
            .GroupBy(e => e.OccurredAtUtc);
        
        MemoryStream dataStream = new();
        await using var writer  = new StreamWriter(dataStream, leaveOpen: true);
        await using var csv     = new CsvWriter(writer, CultureInfo.InvariantCulture);

        var csvTemplate = template.GetData<CsvExportTemplate>();
        if (csvTemplate is null)
        {
            throw new InvalidOperationException
            (
                $"Template '{template.Id}' does not contain {nameof(CsvExportTemplate)} as data."
            );
        }

        foreach (string header in csvTemplate.Headers)
            csv.WriteField<string>(header);

        await csv.NextRecordAsync();

        foreach (IGrouping<DateTime, Event> group in groupedEvents)
        {
            // TODO: this is bad and incorrect. Need to be able to select a "primary"
            // tag instead of simply choosing the first one.
            IDictionary<string, Event> taggedEvents = group.ToDictionary
            (
                e => e.Tags.FirstOrDefault()?.Name,
                e => e
            );
            
            foreach (string header in csvTemplate.Headers) 
            {
                if (!taggedEvents.TryGetValue(header, out Event @event)) continue;
                csv.WriteField(@event.Text);
            }    
            
            await csv.NextRecordAsync();
        }

        await csv.FlushAsync();

        return dataStream.ToArray();
    }
}