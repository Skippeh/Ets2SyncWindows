using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Ets2SyncWindows.Controls
{
    public enum AlertMessageType
    {
        Invalid,
        Info,
        Warning,
        Danger,
        Success
    }

    [ContentProperty(nameof(ContentChildren))]
    public partial class AlertMessage : UserControl
    {
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(AlertMessageType), typeof(AlertMessage));
        public static readonly DependencyProperty ContentChildrenProperty = DependencyProperty.Register(nameof(ContentChildren), typeof(UIElementCollection), typeof(AlertMessage));

        public AlertMessageType Type
        {
            get => (AlertMessageType) GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public UIElementCollection ContentChildren
        {
            get => (UIElementCollection) GetValue(ContentChildrenProperty);
            set => SetValue(ContentChildrenProperty, value);
        }

        public AlertMessage()
        {
            InitializeComponent();
            ContentChildren = ContentPanel.Children;
        }
    }
}