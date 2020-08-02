using System.IO;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using PrismLibrary.Exceptions;
using PrismLibrary.PrismData;

namespace PrismLibrary
{
    public static class PrismEncryption
    {
        private static readonly byte[] Key =
        {
            0x2A, 0x5F, 0xCB, 0x17, 0x91, 0xD2, 0x2F, 0xB6,
            0x02, 0x45, 0xB3, 0xD8, 0x36, 0x9E, 0xD0, 0xB2,
            0xC2, 0x73, 0x71, 0x56, 0x3F, 0xBF, 0x1F, 0x3C,
            0x9E, 0xDF, 0x6B, 0x11, 0x82, 0x5A, 0x5D, 0x0A
        };
        
        public static byte[] DecryptAndDecompressFile(FileStream stream)
        {
            if (stream.Length < 4)
                throw new SiiFormatException(SiiFormatException.ParseError.InvalidSize);

            stream.Seek(0, SeekOrigin.Begin);

            using var binaryReader = new BinaryReader(stream, Encoding.UTF8, true);
            var signatureChars = binaryReader.ReadChars(4);
            string signature = new string(signatureChars);
            
            if (signature == "SiiN") // File is not encrypted or compressed
            {
                byte[] bytes = binaryReader.ReadBytes((int) stream.Length - 4);
                return bytes;
            }
            
            if (signature != "ScsC") // ScsC is the only valid alternative (binary is not supported).
                throw new SiiFormatException(SiiFormatException.ParseError.InvalidSignature);
            
            ScsCHeader scscHeader = ScsCHeader.DeserializeFromStream(stream);

            byte[] decryptedBytes = new byte[scscHeader.DataSize];
            
            using (Aes aes = AesManaged.Create())
            {
                aes.IV = scscHeader.InitVector;
                aes.Key = Key;
                aes.Mode = CipherMode.CBC;

                using ICryptoTransform decryptor = aes.CreateDecryptor();
                using CryptoStream cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read, true);
                cryptoStream.Read(decryptedBytes);
            }

            using (var outputMemoryStream = new MemoryStream())
            using (var memoryStream = new MemoryStream(decryptedBytes))
            using (var inflater = new InflaterInputStream(memoryStream))
            {
                inflater.CopyTo(outputMemoryStream);

                byte[] decompressedBytes = outputMemoryStream.ToArray();
                return decompressedBytes;
            }
        }
    }
}