using System.Globalization;
using CsvHelper;
using Diagraph.Modules.Events.DataExports.Contracts;

namespace Diagraph.Modules.Events.DataExports.Csv;

public class CsvDataWriter : IDataWriter
{
    public async Task<byte[]> WriteEventAsync(IEnumerable<Event> events)
    {
        var dataStream = (MemoryStream)await WriteEventStreamAsync(events);
        return dataStream.ToArray();
    }

    public async Task<Stream> WriteEventStreamAsync(IEnumerable<Event> events)
    {
        MemoryStream    dataStream = new();
        await using var writer     = new StreamWriter(dataStream, leaveOpen: true);
        await using var csv        = new CsvWriter(writer, CultureInfo.InvariantCulture);
        
        await csv.WriteRecordsAsync(events);
        await csv.FlushAsync();

        return dataStream;
    }
}