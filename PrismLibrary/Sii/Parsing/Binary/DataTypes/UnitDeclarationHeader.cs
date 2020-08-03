using System.Runtime.InteropServices;

namespace PrismLibrary.Sii.Parsing.Binary.DataTypes
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UnitDeclarationHeader
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable FieldCanBeMadeReadOnly.Global
        
        public int Flag;
        public int Format;
        public char Index1;
        public int Index2;
        public int NameLength;
        
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore FieldCanBeMadeReadOnly.Global
    }
}