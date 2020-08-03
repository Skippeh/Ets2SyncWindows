using System.Collections.Generic;

namespace PrismLibrary.Sii.Parsing.Binary.DataTypes
{
    public class UnitValue
    {
        public string Name;
        public UnitDeclaration Type;
        public List<PropertyValue> Values;
    }
}