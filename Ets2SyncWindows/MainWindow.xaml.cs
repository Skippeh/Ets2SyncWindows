using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ets2SyncWindows.Controls;
using Ets2SyncWindows.Data;
using Hardcodet.Wpf.TaskbarNotification;
using PrismLibrary;
using PrismSyncLibrary;
using PrismSyncLibrary.WebEts2Sync;

namespace Ets2SyncWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AppState AppState { get; private set; }

        private readonly PersistentAppState loadedAppState;

        private const string ConfigFilePath = "config.json";

        private bool isShuttingDown;
        
        public MainWindow()
        {
            InitializeComponent();

            loadedAppState = PersistentAppState.LoadOrCreateDefault(ConfigFilePath);
            AppState = new AppState(this);
            loadedAppState.ApplyTo(AppState);
            SaveConfig();

            DataContext = AppState;
        }

        public async Task SyncJobs()
        {
            if (AppState.SyncingJobs || AppState.SelectedSave == null)
                return;

            SaveConfig();
            AppState.SyncingJobs = true;

            if (AppState.BackupSavesBeforeSyncing)
            {
                try
                {
                    await SaveBackupManager.BackupSave(AppState.SelectedSave);
                }
                catch (IOException ex)
                {
                    AppState.MainWindow.ShowTaskBarPopup("Error", $"Could not backup save file:\n{ex.Message}", BalloonIcon.Error);
                    AppState.SyncingJobs = false;
                    return;
                }
            }

            SyncResult syncResult = await PrismSyncManager.SyncSave<WebSynchronizer>(AppState.SelectedSave, AppState.SelectedGame.ModType, GetSelectedDlcBitmask());

            if (syncResult.Result == ResultType.Success)
            {
                AppState.MainWindow.ShowTaskBarPopup("Jobs Synced", "Jobs synced successfully. You can now load the save in-game.", BalloonIcon.Info);
            }
            else
            {
                AppState.MainWindow.ShowTaskBarPopup("Error", $"Could not sync jobs:\n{syncResult.Exception.Message}", BalloonIcon.Error);
            }
            
            AppState.SyncingJobs = false;
        }

        private int GetSelectedDlcBitmask()
        {
            int result = 0;
            var dlcs = AppState.SelectedDlcs[AppState.SelectedGame];

            foreach (Dlc dlc in dlcs.MapDlcs)
                if (dlc.Selected)
                    result |= dlc.Bitmask;
            
            foreach (Dlc dlc in dlcs.CargoDlcs)
                if (dlc.Selected)
                    result |= dlc.Bitmask;
            
            foreach (Dlc dlc in dlcs.TrailerDlcs)
                if (dlc.Selected)
                    result |= dlc.Bitmask;

            return result;
        }

        public void SaveConfig()
        {
            loadedAppState.ReadFrom(AppState);
            loadedAppState.Save(ConfigFilePath);
        }

        public void ShowTaskBarPopup(string title, string message, BalloonIcon symbol)
        {
            TaskBarIcon.ShowBalloonTip(title, message, symbol);
        }

        private async void OnKeyDown(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.F5)
            {
                await SyncJobs();
            }
        }

        private void OnFixConfigClicked(object sender, RoutedEventArgs args)
        {
            PrismConfig gameConfig = AppState.GameConfig;
            if (gameConfig == null)
                return;

            gameConfig["g_save_format"] = "2";
            gameConfig["g_developer"] = "1";
            gameConfig["g_console"] = "1";
            gameConfig.Save();
        }

        private void OnRefreshGameConfigClicked(object sender, RoutedEventArgs args)
        {
            AppState.SelectedGame = AppState.SelectedGame; // Config is reloaded when game is set.
        }

        private void OnWindowClosed(object sender, EventArgs args)
        {
            SaveConfig();
        }

        private void OnWindowClosing(object sender, CancelEventArgs args)
        {
            if (AppState.MinimizeToTaskBar)
            {
                Hide();
                args.Cancel = true;

                if (!isShuttingDown)
                {
                    ShowTaskBarPopup("Minimized to Taskbar", "ATS/ETS2 Job Sync has been minimized to the taskbar.", BalloonIcon.Info);
                }
            }
        }

        private void OnTrayIconMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Show();
            Activate(); // Bring window to front
        }

        public void Shutdown()
        {
            isShuttingDown = true;
            Application.Current.Shutdown();
        }
    }
}