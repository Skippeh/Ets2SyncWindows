using System.Runtime.InteropServices;

namespace PrismLibrary.Sii.Parsing.Binary.DataTypes
{
    [StructLayout(LayoutKind.Explicit)]
    public struct FileHeader
    {
        // ReSharper disable FieldCanBeMadeReadOnly.Global
        // ReSharper disable MemberCanBePrivate.Global
        
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 4)]
        public char[] Signature;
        
        // ReSharper restore FieldCanBeMadeReadOnly.Global
        // ReSharper restore MemberCanBePrivate.Global
    }
}