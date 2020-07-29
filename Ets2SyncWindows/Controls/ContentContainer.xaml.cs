using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Ets2SyncWindows.Controls
{
    [ContentProperty(nameof(ContentContainerChildren))]
    public partial class ContentContainer : UserControl
    {
        public static readonly DependencyProperty ContentContainerChildrenProperty = DependencyProperty.Register(
            nameof(ContentContainerChildren),
            typeof(UIElementCollection),
            typeof(Card)
        );
        
        public UIElementCollection ContentContainerChildren
        {
            get => (UIElementCollection) GetValue(ContentContainerChildrenProperty);
            set => SetValue(ContentContainerChildrenProperty, value);
        }
        
        public ContentContainer()
        {
            InitializeComponent();
            ContentContainerChildren = ContentPanel.Children;
        }
    }
}