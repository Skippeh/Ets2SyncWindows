using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Ets2SyncWindows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnComboBoxRequestBringIntoView(object sender, RequestBringIntoViewEventArgs args)
        {
            // Prevents changing selected item when you use the scroll wheel
            args.Handled = true;
        }
    }
}