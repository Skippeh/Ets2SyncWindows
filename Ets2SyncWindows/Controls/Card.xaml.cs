using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Ets2SyncWindows.Controls
{
    [ContentProperty(nameof(CardChildren))]
    public partial class Card : UserControl
    {
        public static readonly DependencyProperty CardChildrenProperty = DependencyProperty.Register(
            nameof(CardChildren),
            typeof(UIElementCollection),
            typeof(Card)
        );

        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
            nameof(HeaderText),
            typeof(string),
            typeof(Card)
        );

        public static readonly DependencyProperty ExpandedProperty = DependencyProperty.Register(
            nameof(Expanded),
            typeof(bool),
            typeof(Card),
            new UIPropertyMetadata(true)
        );

        public static readonly DependencyProperty CanToggleExpansionProperty = DependencyProperty.Register(
            nameof(CanToggleExpansion),
            typeof(bool),
            typeof(Card),
            new UIPropertyMetadata(true)
        );

        public UIElementCollection CardChildren
        {
            get => (UIElementCollection) GetValue(CardChildrenProperty);
            set => SetValue(CardChildrenProperty, value);
        }

        public string HeaderText
        {
            get => (string) GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        public bool Expanded
        {
            get => (bool) GetValue(ExpandedProperty);
            set => SetValue(ExpandedProperty, value);
        }

        public bool CanToggleExpansion
        {
            get => (bool) GetValue(CanToggleExpansionProperty);
            set => SetValue(CanToggleExpansionProperty, value);
        }

        public Card()
        {
            InitializeComponent();
            CardChildren = ContentPanel.ContentContainerChildren;
        }

        private void ToggleExpanded(object sender, RoutedEventArgs e)
        {
            if (!CanToggleExpansion)
                return;
            
            Expanded = !Expanded;
        }
    }
}