using System;
using System.IO;
using System.Threading.Tasks;
using Ets2SyncWebApi;
using Ets2SyncWebApi.Responses;
using PrismLibrary;

namespace PrismSyncLibrary.WebEts2Sync
{
    public sealed class WebSynchronizer : IJobSynchronizer
    {
        public async Task<SyncResult> SyncJobsAsync(GameSave gameSave, GameModType modType, int selectedDlc)
        {
            var webApi = new Ets2SyncApiHost();
            var syncJobsResult = await webApi.SyncJobs(gameSave, modType, selectedDlc);

            if (syncJobsResult.Result != SyncJobsResult.SyncResult.Success)
            {
                return SyncResult.Fail(syncJobsResult.ErrorException);
            }

            await using var fileWriter = File.Create(gameSave.FilePath);
            await fileWriter.WriteAsync(syncJobsResult.SaveFileBytes);
            return SyncResult.Success(syncJobsResult.SaveFileBytes);
        }
    }
}