using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using PrismLibrary;

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
            if (AppState.SyncingJobs)
                return;

            SaveConfig();
            AppState.SyncingJobs = true;
            await Task.Delay(1000);
            AppState.SyncingJobs = false;
        }

        public void SaveConfig()
        {
            loadedAppState.ReadFrom(AppState);
            loadedAppState.Save(ConfigFilePath);
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
    }
}