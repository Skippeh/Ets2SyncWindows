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

        public Card()
        {
            InitializeComponent();
            CardChildren = ContentPanel.ContentContainerChildren;
        }
    }
}