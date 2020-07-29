using System;
using System.ComponentModel;
using System.Reflection;
using Ets2SyncWindows.Annotations;
using Ets2SyncWindows.Data;

namespace Ets2SyncWindows
{
    public static class EnumUtility
    {
        public static string GetNameOfValue([NotNull] object value, [NotNull] Type enumType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType));

            if (!enumType.IsEnum)
                throw new ArgumentException("Specified type is not an enum.", nameof(enumType));

            string enumName = Enum.GetName(enumType, value);

            if (enumName == null)
                return null;
            
            var field = enumType.GetField(enumName);

            if (field != null)
            {
                var descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>();

                if (descriptionAttribute != null)
                    enumName = descriptionAttribute.Description;
            }

            return enumName;
        }

        public static bool HasIgnoreAttribute(object value, Type enumType)
        {
            var enumName = Enum.GetName(enumType, value);
            var field = enumType.GetField(enumName);

            return field?.GetCustomAttribute<IgnoreAttribute>() != null;
        }
    }
}