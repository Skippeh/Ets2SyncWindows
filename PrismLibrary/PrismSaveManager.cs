using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace PrismLibrary
{
    public static class PrismSaveManager
    {
        private static bool listenForGameSaves;
        private static readonly Dictionary<GameProfile, FileSystemWatcher> GameSaveWatchers = new Dictionary<GameProfile, FileSystemWatcher>();

        public delegate void GameSavedEventHandler(GameSavedEventArgs eventArgs);

        public static event GameSavedEventHandler GameSaved;

        public static bool ListenForGameSaves
        {
            get => listenForGameSaves;
            set
            {
                if (listenForGameSaves == value)
                    return;
                
                listenForGameSaves = value;

                if (value)
                {
                    var allProfiles = GetAllProfiles(GameType.Ats, false).Concat(GetAllProfiles(GameType.Ets2, false));
                    
                    foreach (GameProfile profile in allProfiles)
                    {
                        string watchFilePath = Path.Combine(profile.RootFilePath);
                        
                        var watcher = new FileSystemWatcher
                        {
                            Path = watchFilePath,
                            Filter = "*game.sii",
                            IncludeSubdirectories = true,
                            EnableRaisingEvents = true
                        };
                        
                        watcher.Changed += OnSaveFileChanged;
                        watcher.Created += OnSaveFileChanged;
                        watcher.Renamed += OnSaveFileChanged;
                        GameSaveWatchers.Add(profile, watcher);
                    }
                }
                else
                {
                    foreach (var watcher in GameSaveWatchers.Values)
                    {
                        watcher.EnableRaisingEvents = false;
                        watcher.Dispose();
                    }

                    GameSaveWatchers.Clear();
                }
            }
        }

        private static void OnSaveFileChanged(object sender, FileSystemEventArgs args)
        {
            if (args.ChangeType != WatcherChangeTypes.Created && args.ChangeType != WatcherChangeTypes.Changed && args.ChangeType != WatcherChangeTypes.Renamed)
                return;
            
            string directoryPath = Path.GetDirectoryName(args.FullPath);
            var profilePath = Path.GetFullPath(Path.Combine(directoryPath, "../..")); // The profile path is 2 directories up from the save directory

            string steamPath = SteamUtility.GetSteamInstallDirectory();
            
            ProfileType profileType = PathUtility.IsPathContainedIn(profilePath, steamPath) ? ProfileType.SteamCloud : ProfileType.Local;
            GameProfile gameProfile = ParseGameProfile(profilePath, profileType, false);
            GameSave gameSave = ParseGameSave(directoryPath);

            if (gameSave != null)
                OnGameSaved(gameProfile, gameSave);
        }

        private static void OnGameSaved(GameProfile profile, GameSave save)
        {
            GameSaved?.Invoke(new GameSavedEventArgs(profile, save));
        }

        public static IEnumerable<GameProfile> GetAllProfiles(GameType gameType, bool readAllSaves = true)
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
                        var gameProfile = ParseGameProfile(profilePath, ProfileType.SteamCloud, readAllSaves);

                        if (gameProfile != null)
                            yield return gameProfile;
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
                    var gameProfile = ParseGameProfile(profilePath, ProfileType.Local, readAllSaves);

                    if (gameProfile != null)
                        yield return gameProfile;
                }
            }
        }

        private static GameProfile ParseGameProfile(string profilePath, ProfileType profileType, bool readAllSaves = true)
        {
            var profileDataPath = Path.Combine(profilePath, "profile.sii");

            if (!File.Exists(profileDataPath))
                return null;
            
            using var fileStream = FileUtility.WaitForFile(profileDataPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1000).Result;
            byte[] decryptedBytes = PrismEncryption.DecryptAndDecompressFile(fileStream);
            string bytesAsString = Encoding.UTF8.GetString(decryptedBytes);

            GameProfile result = new GameProfile(profileType)
            {
                RootFilePath = profilePath
            };

            result.GameType = GetGameTypeFromPathAndProfileType(profilePath, profileType);

            foreach (var kv in PrismUtility.ParseSiiTextFile(bytesAsString))
            {
                if (kv.Key == "profile_name")
                    result.Name = kv.Value;

                if (kv.Key == "save_time")
                    result.LastSaveTime = TimeUtility.EpochToDateTime(long.Parse(kv.Value));
            }

            if (readAllSaves)
                result.Saves = new ObservableCollection<GameSave>(ParseGameSaves(result.RootFilePath).OrderByDescending(save => save.SaveTime));
            
            return result;
        }

        private static IEnumerable<GameSave> ParseGameSaves(string profilePath)
        {
            string savesPath = Path.Combine(profilePath, "save");

            if (!Directory.Exists(savesPath))
                yield break;

            foreach (string saveDirectoryPath in Directory.EnumerateDirectories(savesPath))
            {
                var gameSave = ParseGameSave(saveDirectoryPath);

                if (gameSave != null)
                    yield return gameSave;
            }
        }

        private static GameSave ParseGameSave(string saveDirectoryPath)
        {
            string infoFilePath = Path.Combine(saveDirectoryPath, "info.sii");
            string gameFilePath = Path.Combine(saveDirectoryPath, "game.sii");
            string thumbnailFilePath = Path.Combine(saveDirectoryPath, "preview.tga");

            if (!File.Exists(infoFilePath) || !File.Exists(gameFilePath))
                return null;

            using var fileStream = FileUtility.WaitForFile(infoFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1000).Result;
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

            if (result.Name.Contains("@@noname_save_game@@") || string.IsNullOrEmpty(result.Name))
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

                if (result.Name.Contains("@@noname_save_game@@"))
                    result.Name = result.Name.Replace("@@noname_save_game@@", saveName);
                else
                    result.Name = saveName;
            }

            return result;
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

        private static GameType GetGameTypeFromPathAndProfileType(string profilePath, ProfileType profileType)
        {
            string normalizedPath = profilePath.Replace("\\", "/");
            switch (profileType)
            {
                case ProfileType.Local:
                {
                    if (normalizedPath.Contains("/Euro Truck Simulator 2/"))
                        return GameType.Ets2;

                    if (normalizedPath.Contains("/American Truck Simulator/"))
                        return GameType.Ats;
                    
                    break;
                }
                case ProfileType.SteamCloud:
                {
                    if (normalizedPath.Contains("/227300/"))
                        return GameType.Ets2;

                    if (normalizedPath.Contains("/270880/"))
                        return GameType.Ats;

                    break;
                }
            }
            
            throw new NotImplementedException($"Could not determine game type from profile path and type: {profilePath} - {profileType}");
        }
    }
}