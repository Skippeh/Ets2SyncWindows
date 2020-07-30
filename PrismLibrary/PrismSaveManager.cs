using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace PrismLibrary
{
    public static class PrismSaveManager
    {
        public static IEnumerable<GameProfile> GetAllProfiles(GameType gameType)
        {
            foreach (KeyValuePair<string, string> kv in SteamUtility.GetAppIdCloudDataPath(GetAppId(gameType)))
            {
                string steamId3 = kv.Key;
                string appIdPath = kv.Value;
                string profilesPath = Path.Combine(appIdPath, "profiles");

                if (Directory.Exists(profilesPath))
                {
                    foreach (string profilePath in Directory.EnumerateDirectories(profilesPath))
                    {
                        yield return ParseGameProfile(profilePath, ProfileType.SteamCloud);
                    }
                }
            }

            string documentsProfilePath = Path.Combine(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    PathUtility.GetGameDocumentsFolderName(gameType),
                    "profiles")
            );

            if (Directory.Exists(documentsProfilePath))
            {
                foreach (string profilePath in Directory.EnumerateDirectories(documentsProfilePath))
                {
                    yield return ParseGameProfile(profilePath, ProfileType.Local);
                }
            }
        }

        private static GameProfile ParseGameProfile(string profilePath, ProfileType profileType)
        {
            var profileDataPath = Path.Combine(profilePath, "profile.sii");

            if (!File.Exists(profileDataPath))
                return null;
            
            using var fileStream = File.OpenRead(profileDataPath);
            byte[] decryptedBytes = PrismEncryption.DecryptAndDecompressFile(fileStream);
            string bytesAsString = Encoding.UTF8.GetString(decryptedBytes);

            GameProfile result = new GameProfile(profileType)
            {
                RootFilePath = profilePath
            };

            foreach (var kv in PrismUtility.ParseSiiTextFile(bytesAsString))
            {
                if (kv.Key == "profile_name")
                    result.Name = kv.Value;

                if (kv.Key == "save_time")
                    result.LastSaveTime = TimeUtility.EpochToDateTime(long.Parse(kv.Value));
            }

            result.Saves = ParseGameSaves(result.RootFilePath).OrderByDescending(save => save.SaveTime).ToArray();
            return result;
        }

        private static IEnumerable<GameSave> ParseGameSaves(string profilePath)
        {
            string savesPath = Path.Combine(profilePath, "save");

            if (!Directory.Exists(savesPath))
                yield break;

            foreach (string saveDirectoryPath in Directory.EnumerateDirectories(savesPath))
            {
                string infoFilePath = Path.Combine(saveDirectoryPath, "info.sii");
                string gameFilePath = Path.Combine(saveDirectoryPath, "game.sii");
                string thumbnailFilePath = Path.Combine(saveDirectoryPath, "preview.tga");

                if (!File.Exists(infoFilePath) || !File.Exists(gameFilePath))
                    continue;

                using var fileStream = File.OpenRead(infoFilePath);
                byte[] bytes = PrismEncryption.DecryptAndDecompressFile(fileStream);
                string bytesAsString = Encoding.UTF8.GetString(bytes);
                var result = new GameSave
                {
                    FilePath = gameFilePath,
                    ThumbnailPath = File.Exists(thumbnailFilePath) ? thumbnailFilePath : null
                };

                {
                    string directoryName = new DirectoryInfo(saveDirectoryPath).Name;
                    string lowerInvariant = directoryName.ToLowerInvariant();
                    
                    if (lowerInvariant.Contains("quicksave"))
                    {
                        result.SaveType = GameSaveType.Quick;
                    }
                    else if (lowerInvariant.Contains("autosave"))
                    {
                        result.SaveType = GameSaveType.Auto;
                    }
                    else
                    {
                        result.SaveType = GameSaveType.Manual;
                    }
                }

                foreach (var kv in PrismUtility.ParseSiiTextFile(bytesAsString))
                {
                    if (kv.Key == "name")
                        result.Name = kv.Value;
                    else if (kv.Key == "file_time")
                        result.SaveTime = TimeUtility.EpochToDateTime(long.Parse(kv.Value));
                }

                if (result.Name.ToLower().Contains("@@noname_save_game@@") || string.IsNullOrEmpty(result.Name))
                {
                    string saveName = "Unnamed save";

                    if (result.SaveType == GameSaveType.Quick)
                    {
                        saveName = "Quicksave";
                    }
                    else if (result.SaveType == GameSaveType.Auto)
                    {
                        saveName = "Autosave";
                    }
                    
                    result.Name = saveName;
                }

                yield return result;
            }
        }

        private static ulong GetAppId(GameType gameType)
        {
            switch (gameType)
            {
                case GameType.Ets2: return 227300;
                case GameType.Ats: return 270880;
            }

            throw new NotImplementedException();
        }
    }
}