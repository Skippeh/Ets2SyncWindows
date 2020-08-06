using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PrismLibrary;
using PrismLibrary.Sii;
using PrismLibrary.Sii.Parsing;
using PrismLibrary.Sii.Parsing.Binary;
using PrismLibrary.Sii.Parsing.Text;
using PrismLibrary.Sii.Serializing;
using PrismLibrary.Sii.Serializing.Binary;

namespace PrismSyncLibrary.OfflineSync
{
    public class OfflineSynchronizer : IJobSynchronizer
    {
        public async Task<SyncResult> SyncJobsAsync(GameSave gameSave, GameModType modType, int selectedDlc)
        {
            await using (FileStream file = File.OpenRead(gameSave.FilePath))
            {
                var bytes = PrismEncryption.DecryptAndDecompressFile(file);
                await File.WriteAllBytesAsync(Path.ChangeExtension(gameSave.FilePath, ".sii.raw.bin"), bytes);

                // Parse binary sii file and write it again to disk
                await using var memoryStream = new MemoryStream(bytes, false);
                var siiFile = SiiParsing.ParseStream<BinarySIIParser>(memoryStream);
                await using (var file2 = File.Create(Path.ChangeExtension(gameSave.FilePath, ".sii.raw.bin.serialized")))
                {
                    SiiSerializing.SerializeSiiFile<BinarySIISerializer>(siiFile, file2);
                }
            }

            if (File.Exists(Path.ChangeExtension(gameSave.FilePath, ".sii.0")))
            {
                await using FileStream file = File.OpenRead(Path.ChangeExtension(gameSave.FilePath, ".sii.0"));
                var bytes = PrismEncryption.DecryptAndDecompressFile(file);
                string siiText = Encoding.UTF8.GetString(bytes);
                await File.WriteAllTextAsync(Path.ChangeExtension(gameSave.FilePath, ".sii.raw.txt"), siiText);

                await using var memoryStream = new MemoryStream(bytes, false);
                var siiFile = SiiParsing.ParseStream<TextSIIParser>(memoryStream);
            }

            throw new NotImplementedException("Sync jobs offline not implemented");
        }
    }
}