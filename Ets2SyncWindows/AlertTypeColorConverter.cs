using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Ets2SyncWindows.Controls;

namespace Ets2SyncWindows
{
    public class AlertTypeToBrushConverter : IValueConverter
    {
        private static readonly Brush InvalidBrush = CreateBackgroundFillBrush(Colors.Purple);
        private static readonly Brush InfoBrush = CreateBackgroundFillBrush(Color.FromRgb(209, 236, 241));
        private static readonly Brush WarningBrush = CreateBackgroundFillBrush(Color.FromRgb(255, 243, 205));
        private static readonly Brush DangerBrush = CreateBackgroundFillBrush(Color.FromRgb(248, 215, 218));
        private static readonly Brush SuccessBrush = CreateBackgroundFillBrush(Color.FromRgb(212, 237, 218));

        private static Brush CreateBackgroundFillBrush(Color mainColor)
        {
            return new SolidColorBrush(mainColor);
        }

        private Brush GetBrush(AlertMessageType type)
        {
            switch (type)
            {
                case AlertMessageType.Invalid: return InvalidBrush;
                case AlertMessageType.Info: return InfoBrush;
                case AlertMessageType.Warning: return WarningBrush;
                case AlertMessageType.Danger: return DangerBrush;
                case AlertMessageType.Success: return SuccessBrush;
            }

            throw new NotImplementedException();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetBrush((AlertMessageType) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}