using System;
using System.Collections;
using System.Reflection;
using System.Windows.Data;

namespace Ets2SyncWindows
{
    public class DictionaryItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null && values.Length >= 2)
            {
                if (values[0] is IDictionary myDict && values[1] != null)
                {
                    object currentObject = myDict[values[1]];

                    for (int i = 2; i < values.Length; ++i)
                    {
                        if (currentObject == null)
                            return Binding.DoNothing;

                        if (!(values[i] is string propertyName))
                            throw new ArgumentException("All values after the first 2 values need to be strings.");
                        
                        Type currentObjectType = currentObject.GetType();
                        PropertyInfo propertyInfo = currentObjectType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                        currentObject = propertyInfo?.GetValue(currentObject);
                    }
                    
                    return currentObject;
                }
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}