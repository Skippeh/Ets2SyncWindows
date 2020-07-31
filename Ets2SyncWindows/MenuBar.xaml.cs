using System.Windows;
using System.Windows.Controls;

namespace Ets2SyncWindows
{
    public partial class MenuBar : UserControl
    {
        public MenuBar()
        {
            InitializeComponent();
        }

        private void OnExitClicked(object sender, RoutedEventArgs e)
        {
            this.FindParent<MainWindow>().Shutdown();
        }
    }
}