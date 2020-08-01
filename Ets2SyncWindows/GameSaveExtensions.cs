using System.IO;
using System.Threading.Tasks;
using PrismLibrary;

namespace Ets2SyncWindows
{
    public static class GameSaveBackupExtensions
    {
        public static bool DoesBackupExist(this GameSave gameSave)
        {
            string backupFilePath = gameSave.FilePath + ".backup";
            return File.Exists(backupFilePath);
        }

        public static async Task<IOException> RestoreBackup(this GameSave gameSave)
        {
            if (!DoesBackupExist(gameSave))
                return new FileNotFoundException("Backup file does not exist.");

            return await Task.Run(() =>
            {
                string backupFilePath = gameSave.FilePath + ".backup";

                try
                {
                    File.Delete(gameSave.FilePath);
                    File.Copy(backupFilePath, gameSave.FilePath);
                    File.Delete(backupFilePath);
                    return null;
                }
                catch (IOException ex)
                {
                    return ex;
                }
            });
        }
    }
}