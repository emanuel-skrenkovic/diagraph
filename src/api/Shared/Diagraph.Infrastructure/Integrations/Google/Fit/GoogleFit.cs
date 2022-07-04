using System.Net.Http.Json;
using Diagraph.Infrastructure.Time.Extensions;
using Google.Apis.Fitness.v1.Data;
using DataPoint = Diagraph.Infrastructure.Integrations.Google.Fit.Contracts.DataPoint;
using Dataset = Diagraph.Infrastructure.Integrations.Google.Fit.Contracts.Dataset;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable CollectionNeverUpdated.Global

namespace Diagraph.Infrastructure.Integrations.Google.Fit;

public class GoogleFit
{
    private const string DataStreamName = "merge_activity_segments";
    
    private readonly IHttpClientFactory _clientFactory;

    public GoogleFit(IHttpClientFactory clientFactory)
        => _clientFactory = clientFactory;

    public async Task<IEnumerable<DataPoint>> GetActivitiesAsync(DateTime? from, DateTime? to)
    {
        HttpClient client = _clientFactory.CreateClient(GoogleIntegrationConsts.BaseClientName);

        var dataSources = await client.GetFromJsonAsync<ListDataSourcesResponse>
        (
            "/fitness/v1/users/me/dataSources"
        );

        string dataSourceId = dataSources
            ?.DataSource
            ?.FirstOrDefault(ds => ds.DataStreamName == DataStreamName)
            ?.DataStreamId;
        
        if (string.IsNullOrWhiteSpace(dataSourceId))
        {
            // TODO: this is sucks
            throw new InvalidOperationException
            (
                $"Can not get data points without the \"{DataStreamName}\" data stream."
            );
        }
        
        long searchStartNanos = from?.ToUnixTimeNanoseconds() ?? 0;
        long searchEndNanos   = to?.ToUnixTimeNanoseconds() ?? long.MaxValue - 1;
 
        var dataset = await client.GetFromJsonAsync<Dataset>
        (
            $"/fitness/v1/users/me/dataSources/{dataSourceId}/datasets/{searchStartNanos}-{searchEndNanos}"
        );
        
        return dataset?.Point;
    }
}