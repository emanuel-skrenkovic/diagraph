using System.Collections.Specialized;
using System.Net.Http.Json;
using System.Web;
using Google.Apis.Fitness.v1.Data;

namespace Diagraph.Infrastructure.Integrations.Google.Fit;

public class GoogleFitApi
{
    private readonly IHttpClientFactory _clientFactory;

    public GoogleFitApi(IHttpClientFactory clientFactory)
        => _clientFactory = clientFactory;
    
    public async Task<IEnumerable<Session>> GetDataAsync(DateTime? from, DateTime? to) 
    { 
        HttpClient client = _clientFactory.CreateClient(GoogleIntegrationConsts.AuthenticatedClientName);

        List<Session> sessions                    = new();
        ListSessionsResponse listSessionsResponse = null;

        do
        {
            NameValueCollection query = HttpUtility.ParseQueryString("");

            if (from.HasValue)
                query.Add("startTime", from.Value.ToString("TODO"));
            
            if (to.HasValue)
                query.Add("endTime", to.Value.ToString("TODO"));

            if (!string.IsNullOrWhiteSpace(listSessionsResponse?.NextPageToken))
                query.Add("pageToken", listSessionsResponse.NextPageToken);
            
            listSessionsResponse = await client.GetFromJsonAsync<ListSessionsResponse>
            (
                // TODO: format the *optional* dates
                $"/users/@me/sessions{query}"
            );

            if (listSessionsResponse?.Session?.Any() == true)
            {
                sessions = sessions.Concat(listSessionsResponse.Session).ToList();
            }
        } while (listSessionsResponse?.HasMoreData == true);

        return sessions;
    }
}