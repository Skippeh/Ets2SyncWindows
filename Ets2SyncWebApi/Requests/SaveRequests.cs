using System.IO;
using System.Net;
using System.Threading.Tasks;
using Ets2SyncWebApi.Responses;
using PrismLibrary;
using PrismSyncLibrary;
using RestSharp;

namespace Ets2SyncWebApi.Requests
{
    internal static class SaveRequests
    {
        public static async Task<SyncJobsResult> SyncJobs(RestClient client, GameSave gameSave, GameModType modType, int selectedDlc)
        {
            selectedDlc |= 1; // api expects dlc to have 1 set
            
            var request = new RestRequest("save_upload");
            // ReSharper disable once AccessToDisposedClosure
            request.AddFile("savefile", gameSave.FilePath);
            request.AddParameter("game", GetGameParameter(modType));
            request.AddParameter("dlc", selectedDlc);

            var response = await client.ExecuteAsync(request, Method.POST);

            if (!response.IsSuccessful)
            {
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    return new SyncJobsResult
                    {
                        Result = SyncJobsResult.SyncResult.InternalServerError,
                        ErrorException = response.ErrorException
                    };
                }

                return new SyncJobsResult
                {
                    Result = SyncJobsResult.SyncResult.UnknownError,
                    ErrorException = response.ErrorException
                };
            }

            var result = new SyncJobsResult
            {
                Result = SyncJobsResult.SyncResult.Success,
                SaveFileBytes = response.RawBytes
            };

            return result;
        }

        private static int GetGameParameter(GameModType modType)
        {
            // They match at the moment so just return the integer value of the enum.
            return (int) modType;
        }
    }
}