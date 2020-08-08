using System;

namespace PrismLibrary.Sii.Attributes
{
    public class SiiUnitAttribute : Attribute
    {
        public readonly string UnitName;
        
        public SiiUnitAttribute(string unitName)
        {
            UnitName = unitName;
        }
    }
}