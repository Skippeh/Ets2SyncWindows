using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using MomentSharp;
using MomentSharp.Globalization.Languages;

namespace Ets2SyncWindows
{
    public class DateToTimeAgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime dateTime))
                throw new ArgumentException("Specified value is not a DateTime.", nameof(value));

            string result = dateTime.Moment().Calendar();

            if (parameter is string strParameter && strParameter == "initialLowerCase")
            {
                result = result[0].ToString().ToLower() + result.Substring(1);
            }
            
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}