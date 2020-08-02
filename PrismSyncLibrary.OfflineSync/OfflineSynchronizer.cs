using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PrismLibrary;

namespace PrismSyncLibrary.OfflineSync
{
    public class OfflineSynchronizer : IJobSynchronizer
    {
        public async Task<SyncResult> SyncJobsAsync(GameSave gameSave, GameModType modType, int selectedDlc)
        {
            string siiText;
            
            using (var file = File.OpenRead(gameSave.FilePath))
            {
                var bytes = PrismEncryption.DecryptAndDecompressFile(file);
                siiText = Encoding.UTF8.GetString(bytes);

                await File.WriteAllTextAsync(Path.ChangeExtension(gameSave.FilePath, ".sii.raw.txt"), siiText);
            }

            

            throw new NotImplementedException("Not implemented");
        }
    }
}