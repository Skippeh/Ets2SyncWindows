using System.Runtime.InteropServices;

namespace PrismLibrary.Sii.Parsing.Binary.DataTypes
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MaybeKeyValueHeader
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable FieldCanBeMadeReadOnly.Global

        public int Value;
        public int NameLength;

        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore FieldCanBeMadeReadOnly.Global
    }
}