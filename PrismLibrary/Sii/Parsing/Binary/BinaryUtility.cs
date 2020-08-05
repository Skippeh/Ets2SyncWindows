using System;
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

        public static byte[] StructureToByteArray<T>(T structure, int sizeOfT) where T : struct
        {
            unsafe
            {
                byte[] bytes = new byte[sizeOfT];
                Span<byte> result = new Span<byte>(bytes);

                fixed (byte* resultAddr = result)
                {
                    Marshal.StructureToPtr(structure, (IntPtr) resultAddr, false);
                }

                return bytes;
            }
        }

        public static T ReadStructure<T>(this BinaryReader reader) where T : struct => ReadStructure<T>(reader, Marshal.SizeOf<T>());

        public static T ReadStructure<T>(this BinaryReader reader, int sizeOfT) where T : struct
        {
            byte[] bytes = reader.ReadBytes(sizeOfT);
            return ByteArrayToStructure<T>(bytes);
        }

        public static void WriteStructure<T>(this BinaryWriter writer, T structure) where T : struct => WriteStructure<T>(writer, structure, Marshal.SizeOf<T>());

        public static void WriteStructure<T>(this BinaryWriter writer, T structure, int sizeOfT) where T : struct
        {
            byte[] bytes = StructureToByteArray(structure, sizeOfT);
            writer.Write(bytes);
        }

        public static void WriteCharString(this BinaryWriter writer, string val, bool writeLength = false)
        {
            if (writeLength)
                writer.Write((uint) val.Length);

            foreach (char ch in val)
            {
                writer.Write((byte) ch);
            }
        }
    }
}