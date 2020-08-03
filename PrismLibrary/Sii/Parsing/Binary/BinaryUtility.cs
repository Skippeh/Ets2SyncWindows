using System.IO;
using System.Runtime.InteropServices;

namespace PrismLibrary.Sii.Parsing.Binary
{
    internal static class BinaryUtility
    {
        public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            T result;
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            
            try
            {
                result = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }

            return result;
        }

        public static T ReadStructure<T>(this BinaryReader reader) where T : struct => ReadStructure<T>(reader, Marshal.SizeOf<T>());

        public static T ReadStructure<T>(this BinaryReader reader, int sizeOfT) where T : struct
        {
            byte[] bytes = reader.ReadBytes(sizeOfT);
            return ByteArrayToStructure<T>(bytes);
        }
    }
}