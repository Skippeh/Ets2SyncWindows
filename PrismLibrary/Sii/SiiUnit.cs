using System.Collections.Generic;
using PrismLibrary.Sii.Parsing.Binary.DataTypes;

namespace PrismLibrary.Sii
{
    public class SiiUnit
    {
        public string Name { get; set; }
        public List<SiiProperty> Properties { get; set; } = new List<SiiProperty>();
        public UnitDeclaration Type { get; set; }

        public override string ToString()
        {
            return $"{Type.Name} : {Name}";
        }
    }
}