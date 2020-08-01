using System;
using System.IO;
using System.Threading.Tasks;
using PrismLibrary;

namespace Ets2SyncWindows
{
    public static class SaveBackupManager
    {
        /// <summary>
        /// Copies the game save to a new file if a backup doesn't already exist.
        /// </summary>
        public static async Task BackupSave(GameSave gameSave)
        {
            string backupFilePath = GetBackupFilePath(gameSave);

            if (!HasBackup(gameSave))
            {
                await Task.Run(() =>
                {
                    File.Copy(gameSave.FilePath, backupFilePath, false);
                });
            }
        }

        public static async Task RestoreBackup(GameSave gameSave)
        {
            string backupFilePath = GetBackupFilePath(gameSave);

            if (!HasBackup(gameSave))
                throw new FileNotFoundException("Backup file does not exist.", backupFilePath);

            await Task.Run(() =>
            {
                File.Delete(gameSave.FilePath);
                File.Move(backupFilePath, gameSave.FilePath);
            });
        }

        public static bool HasBackup(GameSave gameSave)
        {
            return File.Exists(GetBackupFilePath(gameSave));
        }

        private static string GetBackupFilePath(GameSave gameSave)
        {
            return gameSave.FilePath + ".backup";
        }
    }
}