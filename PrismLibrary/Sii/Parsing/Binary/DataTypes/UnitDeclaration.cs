using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PrismLibrary.Sii.Parsing.Binary.DataTypes
{
    public class UnitDeclaration
    {
        public int Index;
        public string Name;
        public readonly List<PropertyDeclaration> Properties = new List<PropertyDeclaration>();

        public override string ToString()
        {
            return $"[{Index}] {Name}";
        }
    }
}