using System;
using PrismLibrary.Sii.Parsing.Binary.DataTypes;

namespace PrismLibrary.Sii.Attributes
{
    public class SiiPropertyAttribute : Attribute
    {
        public readonly string PropertyName;
        public readonly PropertyType Type;

        public SiiPropertyAttribute(string propertyName, PropertyType type)
        {
            PropertyName = propertyName;
            Type = type;
        }
    }
}