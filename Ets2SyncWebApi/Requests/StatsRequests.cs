using System.Threading.Tasks;
using Ets2SyncWebApi.Models;
using Ets2SyncWebApi.Responses;
using RestSharp;

namespace Ets2SyncWebApi.Requests
{
    internal static class StatsRequests
    {
        public static async Task<JobStats> RequestStats(RestClient client)
        {
            var request = new RestRequest("stat");
            var response = await client.ProcessRequest(client.ExecuteGetAsync<JobStatsResponse>(request));
            return response.Data.ToRealType();
        }
    }
}