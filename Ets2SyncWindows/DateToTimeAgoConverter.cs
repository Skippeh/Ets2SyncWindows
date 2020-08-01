using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Ets2SyncWindows
{
    public class DateToTimeAgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime dateTime))
                throw new ArgumentException("Specified value is not a DateTime.", nameof(value));

            string result;
            TimeSpan timeSince = DateTime.UtcNow - dateTime.ToUniversalTime();

            if (timeSince.TotalSeconds < 60)
            {
                result = $"{timeSince.TotalSeconds:0} second{((int) timeSince.TotalSeconds != 1 ? "s" : "")} ago";
            }
            else if (timeSince.TotalMinutes < 60)
            {
                result = $"{timeSince.TotalMinutes:0} minute{((int) timeSince.TotalMinutes != 1 ? "s" : "")} ago";
            }
            else if (timeSince.TotalHours < 24)
            {
                result = $"{timeSince.TotalHours:0} hour{((int) timeSince.TotalHours != 1 ? "s" : "")} ago";
            }
            else if (timeSince.TotalDays < 7)
            {
                result = $"{timeSince.TotalDays:0} day{((int) timeSince.TotalDays != 1 ? "s" : "")} ago";
            }
            else
            {
                result = dateTime.ToShortDateString();
            }

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