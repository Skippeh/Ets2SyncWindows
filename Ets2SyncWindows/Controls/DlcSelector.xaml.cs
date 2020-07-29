using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Ets2SyncWindows.Controls
{
    public partial class DlcSelector : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(DlcSelector)
        );

        public static readonly DependencyProperty DlcCollectionProperty = DependencyProperty.Register(
            nameof(DlcCollection),
            typeof(ObservableCollection<Dlc>),
            typeof(DlcSelector),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnDlcCollectionPropertyChanged)
        );

        private static void OnDlcCollectionPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var newValue = (ObservableCollection<Dlc>) args.NewValue;
            
            if (dependencyObject is DlcSelector dlcSelector)
            {
                dlcSelector.ItemsControlHeader.Visibility = newValue.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                dlcSelector.ItemsControlHeaderNoItems.Visibility = dlcSelector.ItemsControlHeader.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;

                dlcSelector.OnDlcCheckedChanged(dlcSelector, null);
            }
        }

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public ObservableCollection<Dlc> DlcCollection
        {
            get => (ObservableCollection<Dlc>) GetValue(DlcCollectionProperty);
            set => SetValue(DlcCollectionProperty, value);
        }

        private bool changingCheckStates;
        
        public DlcSelector()
        {
            InitializeComponent();
        }

        private void OnSelectAllChecked(object sender, RoutedEventArgs e)
        {
            changingCheckStates = true;
            
            foreach (var dlc in DlcCollection)
            {
                dlc.Selected = true;
            }

            changingCheckStates = false;
        }

        private void OnSelectAllUnchecked(object sender, RoutedEventArgs e)
        {
            changingCheckStates = true;
            
            foreach (var dlc in DlcCollection)
            {
                dlc.Selected = false;
            }

            changingCheckStates = false;
        }

        private void OnDlcCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (changingCheckStates)
                return;
            
            if (AllDlcChecked())
                CheckBoxSelectAll.IsChecked = true;
            else if (NoDlcChecked())
                CheckBoxSelectAll.IsChecked = false;
            else
                CheckBoxSelectAll.IsChecked = null;
        }

        private bool AllDlcChecked() => DlcCollection.All(dlc => dlc.Selected);
        private bool NoDlcChecked() => DlcCollection.All(dlc => !dlc.Selected);
    }
}