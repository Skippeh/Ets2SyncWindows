using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ets2SyncWindows.Controls
{
    public class ConvertGameComboBoxItem : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var comboBox = (ComboBox) ((ComboBoxItem) value)?.Parent;
            throw new NotImplementedException();
        }
    }
}