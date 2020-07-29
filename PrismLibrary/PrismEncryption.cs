using System.IO;
using System.Security.Cryptography;
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
            if (stream.Length < TSIIHeader.SizeOf)
                throw new SiiFormatException(SiiFormatException.ParseError.InvalidSize);

            stream.Seek(0, SeekOrigin.Begin);
            var tsiiHeader = TSIIHeader.DeserializeFromStream(stream);

            if (tsiiHeader.Signature == "SiiN") // File is not encrypted
                throw new SiiFormatException(SiiFormatException.ParseError.NotEncrypted);
            
            if (tsiiHeader.Signature != "ScsC") // ScsC is the only valid alternative.
                throw new SiiFormatException(SiiFormatException.ParseError.InvalidSignature);

            byte[] decryptedBytes = new byte[tsiiHeader.DataSize];
            
            using (Aes aes = AesManaged.Create())
            {
                aes.IV = tsiiHeader.InitVector;
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