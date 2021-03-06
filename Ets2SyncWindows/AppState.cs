using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Ets2SyncWindows.Annotations;
using Ets2SyncWindows.Data;
using PrismLibrary;

namespace Ets2SyncWindows
{
    public class AppState : INotifyPropertyChanged
    {
        private Game selectedGame;
        private Dictionary<Game, GameDlcs> selectedDlcs = new Dictionary<Game, GameDlcs>();
        private PrismConfig gameConfig;
        private ObservableCollection<GameProfile> gameProfiles = new ObservableCollection<GameProfile>();
        private bool loadingGameProfiles = true;
        private GameProfile selectedProfile;
        private GameSave selectedSave;
        private ImageSource thumbnailImage;
        private bool syncingJobs;
        private bool backupSavesBeforeSyncing;
        private bool automaticallySyncSaves;
        private bool selectSaveCardExpanded;
        private bool selectDlcCardExpanded;
        private bool selectGameCardExpanded;
        private bool minimizeToTaskBar;
        
        private FileSystemWatcher configFileWatcher;
        private bool backupExists;

        private readonly CancellationTokenSource backgroundThreadCancellation = new CancellationTokenSource();

        public PrismConfig GameConfig
        {
            get => gameConfig;
            set
            {
                gameConfig = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(GameConfigIsOk));
                OnPropertyChanged(nameof(GameConfigExists));
                InitializeConfigFileWatcher();
            }
        }

        public ObservableCollection<GameProfile> GameProfiles
        {
            get => gameProfiles;
            set
            {
                gameProfiles = value;
                OnPropertyChanged();
            }
        }

        public Game SelectedGame
        {
            get => selectedGame;
            set
            {
                selectedGame = value;
                OnPropertyChanged();
                var (_, loadedConfig) = PrismConfigManager.LoadGameConfig(value.Type);
                GameConfig = loadedConfig;
                Task.Run(ReloadGameProfiles);
            }
        }

        public Dictionary<Game, GameDlcs> SelectedDlcs
        {
            get => selectedDlcs;
            set
            {
                selectedDlcs = value;
                OnPropertyChanged();
            }
        }

        public GameProfile SelectedProfile
        {
            get => selectedProfile;
            set
            {
                selectedProfile = value;
                OnPropertyChanged();
                SelectedSave = value?.Saves.FirstOrDefault();
            }
        }

        public GameSave SelectedSave
        {
            get => selectedSave;
            set
            {
                selectedSave = value;
                OnPropertyChanged();

                ThumbnailImage = value?.TryLoadThumbnailImage();
                BackupExists = value?.DoesBackupExist() ?? false;
            }
        }

        public bool BackupExists
        {
            get => backupExists;
            set
            {
                backupExists = value;
                OnPropertyChanged();
            }
        }

        public ImageSource ThumbnailImage
        {
            get => thumbnailImage;
            set
            {
                thumbnailImage = value;
                OnPropertyChanged();
            }
        }

        public bool LoadingGameProfiles
        {
            get => loadingGameProfiles;
            set
            {
                loadingGameProfiles = value;
                OnPropertyChanged();
            }
        }

        public bool SyncingJobs
        {
            get => syncingJobs;
            set
            {
                syncingJobs = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShouldUiBeEnabled));
            }
        }

        public bool BackupSavesBeforeSyncing
        {
            get => backupSavesBeforeSyncing;
            set
            {
                backupSavesBeforeSyncing = value;
                OnPropertyChanged();
            }
        }

        public bool AutomaticallySyncSaves
        {
            get => automaticallySyncSaves;
            set
            {
                automaticallySyncSaves = value;
                OnPropertyChanged();
            }
        }

        public bool GameConfigIsOk => GameConfigExists && GameConfig["g_save_format"] == "2" && GameConfig["g_developer"] == "1" && GameConfig["g_console"] == "1";

        public bool GameConfigExists => GameConfig != null;

        public bool ShouldUiBeEnabled => !SyncingJobs;

        public bool SelectGameCardExpanded
        {
            get => selectGameCardExpanded;
            set
            {
                selectGameCardExpanded = value;
                OnPropertyChanged();
            }
        }

        public bool SelectDlcCardExpanded
        {
            get => selectDlcCardExpanded;
            set
            {
                selectDlcCardExpanded = value;
                OnPropertyChanged();
            }
        }

        public bool SelectSaveCardExpanded
        {
            get => selectSaveCardExpanded;
            set
            {
                selectSaveCardExpanded = value;
                OnPropertyChanged();
            }
        }

        public bool MinimizeToTaskBar
        {
            get => minimizeToTaskBar;
            set
            {
                minimizeToTaskBar = value;
                OnPropertyChanged();
            }
        }

        public MainWindow MainWindow { get; }

        public AppState(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            
            foreach (Game game in GameData.Games)
            {
                SelectedDlcs.Add(game, new GameDlcs(game));
            }

            PrismSaveManager.GameSaved += OnGameSaved;
            PrismSaveManager.ListenForGameSaves = true;

            Task.Run(BackgroundUpdateThread);
        }

        private async Task BackgroundUpdateThread()
        {
            while (true)
            {
                if (backgroundThreadCancellation.IsCancellationRequested)
                    break;

                Stopwatch sw = Stopwatch.StartNew();
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    OnPropertyChanged(nameof(SelectedProfile));
                    OnPropertyChanged(nameof(SelectedSave));
                });

                sw.Stop();

                if (sw.ElapsedMilliseconds < 1000)
                    await Task.Delay(TimeSpan.FromMilliseconds(1000).Subtract(sw.Elapsed), backgroundThreadCancellation.Token);
            }
        }

        ~AppState()
        {
            backgroundThreadCancellation.Cancel();
        }
        
        private void InitializeConfigFileWatcher()
        {
            configFileWatcher?.Dispose();

            string configPath = PrismConfigManager.GetConfigFilePath(SelectedGame.Type);

            if (!Directory.Exists(Path.GetDirectoryName(configPath)))
                return;
            
            configFileWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(configPath),
                Filter = Path.GetFileName(configPath),
                EnableRaisingEvents = true // this one needs to be set last because of how the property is implemented
            };

            DateTime lastWriteTime = new DateTime();

            async Task OnFileChanged(FileSystemEventArgs args)
            {
                DateTime writeTime = args.ChangeType != WatcherChangeTypes.Deleted ? File.GetLastWriteTime(args.FullPath) : DateTime.UtcNow;

                if (writeTime - lastWriteTime < TimeSpan.FromSeconds(0.1))
                    return;

                lastWriteTime = writeTime;

                if (args.ChangeType != WatcherChangeTypes.Deleted)
                {
                    await using var fileStream = await FileUtility.WaitForFile(args.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1000);
                    (bool loadedSuccessfully, var loadedConfig) = PrismConfigManager.LoadGameConfig(fileStream);
                    GameConfig = loadedConfig;
                }
                else
                {
                    GameConfig = null;
                }
            }
            
            configFileWatcher.Changed += (sender, args) =>
            {
                OnFileChanged(args).Wait();
            };

            configFileWatcher.Deleted += (sender, args) =>
            {
                OnFileChanged(args).Wait();
            };

            configFileWatcher.Created += (sender, args) =>
            {
                OnFileChanged(args).Wait();
            };
        }

        private async Task ReloadGameProfiles()
        {
            LoadingGameProfiles = true;

            try
            {
                await Task.Run(async () =>
                {
                    var newGameProfiles = PrismSaveManager.GetAllProfiles(SelectedGame.Type).ToList();

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        GameProfiles.Clear();

                        foreach (var gameProfile in newGameProfiles)
                        {
                            GameProfiles.Add(gameProfile);
                        }
                    }).Task;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            SelectedProfile = GameProfiles.OrderByDescending(g => g.LastSaveTime).FirstOrDefault();
            LoadingGameProfiles = false;
        }

        private void OnGameSaved(GameSavedEventArgs args)
        {
            if (args.Profile.GameType != SelectedGame.Type)
                return;
            
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                LoadingGameProfiles = true;

                SelectedProfile = GameProfiles.FirstOrDefault(profile => ArePathsEqual(profile.RootFilePath, args.Profile.RootFilePath));

                if (SelectedProfile == null)
                {
                    await ReloadGameProfiles();
                    return;
                }

                var existingSave = SelectedProfile.Saves.FirstOrDefault(save => save.FilePath.Equals(args.Save.FilePath));

                if (existingSave == null)
                {
                    // New save, assume it's newer than all the previous saves.
                    SelectedProfile.Saves.Insert(0, args.Save);
                    SelectedSave = args.Save;
                }
                else
                {
                    int index = SelectedProfile.Saves.IndexOf(existingSave);
                    SelectedProfile.Saves.RemoveAt(index);
                    SelectedProfile.Saves.Insert(index, args.Save);

                    if (existingSave.FilePath.Equals(args.Save.FilePath))
                        SelectedSave = args.Save;
                }

                if (args.Save.SaveType == GameSaveType.Manual && AutomaticallySyncSaves)
                {
                    await MainWindow.SyncJobs();
                }

                LoadingGameProfiles = false;
            }, DispatcherPriority.Background);
        }

        private bool ArePathsEqual(string pathA, string pathB)
        {
            pathA = pathA.Replace("\\", "/");
            pathB = pathB.Replace("\\", "/");

            return String.Equals(pathA, pathB, StringComparison.InvariantCultureIgnoreCase);
        }

        #region INotifyPropertyChanged interface
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
    }

    public class GameDlcs : INotifyPropertyChanged
    {
        private ObservableCollection<Dlc> trailerDlcs;
        private ObservableCollection<Dlc> cargoDlcs;
        private ObservableCollection<Dlc> mapDlcs;

        public ObservableCollection<Dlc> TrailerDlcs
        {
            get => trailerDlcs;
            set
            {
                trailerDlcs = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Dlc> CargoDlcs
        {
            get => cargoDlcs;
            set
            {
                cargoDlcs = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Dlc> MapDlcs
        {
            get => mapDlcs;
            set
            {
                mapDlcs = value;
                OnPropertyChanged();
            }
        }

        private Game game;

        public GameDlcs(Game game)
        {
            this.game = game;
            MapDlcs = new ObservableCollection<Dlc>();
            CargoDlcs = new ObservableCollection<Dlc>();
            TrailerDlcs = new ObservableCollection<Dlc>();

            InitializeCollection(MapDlcs, typeof(MapDlc));
            InitializeCollection(CargoDlcs, typeof(CargoDlc));
            InitializeCollection(TrailerDlcs, typeof(TrailerDlc));
        }

        private void InitializeCollection(ObservableCollection<Dlc> collection, Type enumType)
        {
            foreach (int value in Enum.GetValues(enumType))
            {
                if (value == 0 || EnumUtility.HasIgnoreAttribute(value, enumType))
                    continue;

                if (enumType == typeof(MapDlc))
                {
                    if (((int) game.MapDlcs & value) == 0)
                        continue;
                }
                else if (enumType == typeof(CargoDlc))
                {
                    if (((int) game.CargoDlcs & value) == 0)
                        continue;
                }
                else if (enumType == typeof(TrailerDlc))
                {
                    if (((int) game.TrailerDlcs & value) == 0)
                        continue;
                }

                string name = EnumUtility.GetNameOfValue(value, enumType);
                collection.Add(new Dlc(name, (int) value));
            }
        }

        #region INotifyPropertyChanged interface
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
    }
    
    public class Dlc : INotifyPropertyChanged
    {
        private string name;
        private int bitmask;
        private bool selected;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public int Bitmask

        {
            get => bitmask;
            set
            {
                bitmask = value;
                OnPropertyChanged();
            }
        }

        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnPropertyChanged();
            }
        }

        public Dlc(string name, int bitmask)
        {
            Name = name;
            Bitmask = bitmask;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}