using System.Threading.Tasks;
using Ets2SyncWebApi.Models;
using Ets2SyncWebApi.Requests;
using Ets2SyncWebApi.Responses;
using PrismLibrary;
using PrismSyncLibrary;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Ets2SyncWebApi
{
    public class Ets2SyncApiHost
    {
        private readonly RestClient client;

        public Ets2SyncApiHost()
        {
            client = new RestClient("https://sync.qqw.ovh");
            client.UseNewtonsoftJson();
        }

        public async Task<JobStats> GetStats()
        {
            return await StatsRequests.RequestStats(client);
        }

        public async Task<SyncJobsResult> SyncJobs(GameSave gameSave, GameModType modType, int selectedDlc)
        {
            return await SaveRequests.SyncJobs(client, gameSave, modType, selectedDlc);
        }
    }
}