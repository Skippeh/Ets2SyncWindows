using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ets2SyncWindows
{
    public static class Theming
    {
        public static readonly DependencyProperty ButtonHoverBackgroundProperty = DependencyProperty.RegisterAttached(
            "ButtonHoverBackground",
            typeof(Brush),
            typeof(Theming),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Purple), FrameworkPropertyMetadataOptions.Inherits)
        );

        public static void SetButtonHoverBackground(DependencyObject dependencyObject, Brush value)
        {
            dependencyObject.SetValue(ButtonHoverBackgroundProperty, value);
        }

        public static Brush GetButtonHoverBackground(DependencyObject dependencyObject)
        {
            return (Brush) dependencyObject.GetValue(ButtonHoverBackgroundProperty);
        }

        public static readonly DependencyProperty ButtonPressedBackgroundProperty = DependencyProperty.RegisterAttached(
            "ButtonPressedBackground",
            typeof(Brush),
            typeof(Theming),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Purple), FrameworkPropertyMetadataOptions.Inherits)
        );

        public static void SetButtonPressedBackground(DependencyObject dependencyObject, Brush value)
        {
            dependencyObject.SetValue(ButtonPressedBackgroundProperty, value);
        }

        public static Brush GetButtonPressedBackground(DependencyObject dependencyObject)
        {
            return (Brush) dependencyObject.GetValue(ButtonPressedBackgroundProperty);
        }

        public static readonly DependencyProperty ButtonDisabledBackgroundProperty = DependencyProperty.RegisterAttached(
            "ButtonDisabledBackground",
            typeof(Brush),
            typeof(Theming),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Purple), FrameworkPropertyMetadataOptions.Inherits)
        );

        public static void SetButtonDisabledBackground(DependencyObject dependencyObject, Brush value)
        {
            dependencyObject.SetValue(ButtonDisabledBackgroundProperty, value);
        }

        public static Brush GetButtonDisabledBackground(DependencyObject dependencyObject)
        {
            return (Brush) dependencyObject.GetValue(ButtonDisabledBackgroundProperty);
        }

        public static readonly DependencyProperty ButtonDisabledForegroundProperty = DependencyProperty.RegisterAttached(
            "ButtonDisabledForeground",
            typeof(Brush),
            typeof(Theming),
            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Purple), FrameworkPropertyMetadataOptions.Inherits)
        );

        public static void SetButtonDisabledForeground(DependencyObject dependencyObject, Brush value)
        {
            dependencyObject.SetValue(ButtonDisabledForegroundProperty, value);
        }

        public static Brush GetButtonDisabledForeground(DependencyObject dependencyObject)
        {
            return (Brush) dependencyObject.GetValue(ButtonDisabledForegroundProperty);
        }
    }
}