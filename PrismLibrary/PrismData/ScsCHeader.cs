using System.IO;
using System.Text;

namespace PrismLibrary.PrismData
{
    public struct ScsCHeader
    {
        /// <summary>
        /// The size of ScsCHeader in bytes.
        /// </summary>
        public const int SizeOf = 56;
        
        public byte[] HMAC; // 32 bytes
        public byte[] InitVector; // 16 bytes
        public uint DataSize;

        public static ScsCHeader DeserializeFromStream(Stream stream)
        {
            var header = new ScsCHeader();
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            header.HMAC = reader.ReadBytes(32);
            header.InitVector = reader.ReadBytes(16);
            header.DataSize = reader.ReadUInt32();

            return header;
        }
    }
}