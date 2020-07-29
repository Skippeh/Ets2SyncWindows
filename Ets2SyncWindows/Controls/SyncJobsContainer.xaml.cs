using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ets2SyncWindows.Controls
{
    public partial class SyncJobsContainer : UserControl
    {
        public SyncJobsContainer()
        {
            InitializeComponent();
        }

        private async void OnSyncJobsClicked(object sender, RoutedEventArgs e)
        {
            var mainWindow = this.FindParent<MainWindow>();

            if (mainWindow != null)
                await mainWindow.SyncJobs();
        }
    }
}