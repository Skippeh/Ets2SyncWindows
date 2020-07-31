using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ets2SyncWindows
{
    public partial class TaskBarContextMenu : ContextMenu
    {
        private AppState AppState => (AppState) DataContext;
        
        public TaskBarContextMenu()
        {
            InitializeComponent();
        }

        private void OnClickOpen(object sender, RoutedEventArgs args)
        {
            AppState.MainWindow.Show();
            AppState.MainWindow.Activate();
        }

        private void OnExitClicked(object sender, RoutedEventArgs args)
        {
            Application.Current.Shutdown();
        }

        private async void OnSyncJobsClicked(object sender, RoutedEventArgs e)
        {
            await AppState.MainWindow.SyncJobs();
        }
    }
}