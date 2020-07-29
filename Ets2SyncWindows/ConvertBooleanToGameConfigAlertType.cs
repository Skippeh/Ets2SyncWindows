using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Ets2SyncWindows.Controls;

namespace Ets2SyncWindows
{
    public class ConvertBooleanToGameConfigAlertType : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool) value)
            {
                return AlertMessageType.Success;
            }

            return AlertMessageType.Warning;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}