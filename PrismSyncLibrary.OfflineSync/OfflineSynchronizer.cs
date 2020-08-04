using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PrismLibrary;
using PrismLibrary.Sii;
using PrismLibrary.Sii.Parsing;
using PrismLibrary.Sii.Parsing.Binary;
using PrismLibrary.Sii.Serializing;
using PrismLibrary.Sii.Serializing.Binary;

namespace PrismSyncLibrary.OfflineSync
{
    public class OfflineSynchronizer : IJobSynchronizer
    {
        public async Task<SyncResult> SyncJobsAsync(GameSave gameSave, GameModType modType, int selectedDlc)
        {
            using (var file = File.OpenRead(gameSave.FilePath))
            {
                var bytes = PrismEncryption.DecryptAndDecompressFile(file);
                await File.WriteAllBytesAsync(Path.ChangeExtension(gameSave.FilePath, ".sii.raw.bin"), bytes);
            }

            if (File.Exists(Path.ChangeExtension(gameSave.FilePath, ".sii.0")))
            {
                using (var file = File.OpenRead(Path.ChangeExtension(gameSave.FilePath, ".sii.0")))
                {
                    var bytes = PrismEncryption.DecryptAndDecompressFile(file);
                    string siiText = Encoding.UTF8.GetString(bytes);
                    await File.WriteAllTextAsync(Path.ChangeExtension(gameSave.FilePath, ".sii.raw.txt"), siiText);
                }
            }

            using var saveStream = File.OpenRead(gameSave.FilePath);
            var siiBytes = PrismEncryption.DecryptAndDecompressFile(saveStream);
            using var memoryStream = new MemoryStream(siiBytes);
            SIIFile siiFile = SiiParsing.ParseStream<BinarySIIParser>(memoryStream);

            using (var file = File.Create(Path.ChangeExtension(gameSave.FilePath, ".sii.raw.bin.serialized")))
            {
                SiiSerializing.SerializeSiiFile<BinarySIISerializer>(siiFile, file);
            }
            
            throw new NotImplementedException();
        }
    }
}