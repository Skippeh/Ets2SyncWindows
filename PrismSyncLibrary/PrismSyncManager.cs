using System;
using System.Threading.Tasks;
using PrismLibrary;

namespace PrismSyncLibrary
{
    public static class PrismSyncManager
    {
        public static Task<SyncResult> SyncSave<T>(GameSave gameSave, GameModType modType, int selectedDlc) where T : IJobSynchronizer
            => SyncSave(typeof(T), gameSave, modType, selectedDlc);

        public static async Task<SyncResult> SyncSave(Type synchronizerType, GameSave gameSave, GameModType modType, int selectedDlc)
        {
            if (!typeof(IJobSynchronizer).IsAssignableFrom(synchronizerType))
                throw new ArgumentException("Specified synchronizer type does not implement IJobSynchronizer.", nameof(synchronizerType));
            
            var syncInstance = (IJobSynchronizer) Activator.CreateInstance(synchronizerType);

            try
            {
                var syncResult = await syncInstance.SyncJobsAsync(gameSave, modType, selectedDlc);
                return syncResult;
            }
            catch (Exception ex)
            {
                return SyncResult.Fail(ex);
            }
        }
    }
}