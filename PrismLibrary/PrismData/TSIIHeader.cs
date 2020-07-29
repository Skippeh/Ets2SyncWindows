using System.IO;
using System.Text;

namespace PrismLibrary.PrismData
{
    public struct TSIIHeader
    {
        /// <summary>
        /// The size of TSIIHeader in bytes.
        /// </summary>
        public const int SizeOf = 56;
            
        public string Signature; // first 4 bytes as characters
        public byte[] HMAC; // 32 bytes
        public byte[] InitVector; // 16 bytes
        public uint DataSize;

        public static TSIIHeader DeserializeFromStream(Stream stream)
        {
            var header = new TSIIHeader();
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            header.Signature = new string(reader.ReadChars(4));
            header.HMAC = reader.ReadBytes(32);
            header.InitVector = reader.ReadBytes(16);
            header.DataSize = reader.ReadUInt32();
            
            return header;
        }
    }
}