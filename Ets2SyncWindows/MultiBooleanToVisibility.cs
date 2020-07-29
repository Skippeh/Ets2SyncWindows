using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ets2SyncWindows
{
    public class MultiBooleanToVisibility : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new ArgumentException("Target type must be Visibility.", nameof(targetType));
            
            foreach (object value in values)
            {
                if (!(value is bool))
                    throw new ArgumentException("Value type must be bool or Nullable<bool>", nameof(values));

                switch (value)
                {
                    case bool boolValue when !boolValue: return Visibility.Collapsed;
                }

                if (Nullable.GetUnderlyingType(value.GetType()) == typeof(bool) && ((bool?) value).GetValueOrDefault(false))
                    return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}