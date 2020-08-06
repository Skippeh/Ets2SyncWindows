using System;
using System.IO;
using System.Linq;
using System.Text;
using PrismLibrary.Sii.Parsing.Text.Exceptions;

namespace PrismLibrary.Sii.Parsing.Text
{
    public class TextSIIParser : ISIIParser
    {
        public SIIFile ParseStream(Stream inputStream)
        {
            // Verify signature
            Span<byte> signatureBytes = new byte[4];
            inputStream.Read(signatureBytes);

            if (!signatureBytes.SequenceEqual(new[] {(byte) 'S', (byte) 'i', (byte) 'i', (byte) 'N'}))
            {
                throw new SiiTextFormatException("File signature does not equal SiiN");
            }

            string streamText;

            // Following code is in a new scope so memStream doesn't exist after assigning streamText.
            {
                if (inputStream is MemoryStream memStream)
                {
                    // If possible avoid allocating the memory needed for the file twice
                    ReadOnlySpan<byte> bytes = memStream.TryGetBuffer(out var bytesSegment) ? bytesSegment.Array : memStream.ToArray();
                    
                    bytes = bytes.Slice(4, bytes.Length - 4);
                    streamText = Encoding.UTF8.GetString(bytes);
                }
                else
                {
                    using var memoryStream = new MemoryStream(1000 * 1000 * 2000); // 2mb initial capacity
                    inputStream.CopyTo(memoryStream);
                    streamText = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }

            var lexer = new SiiLexer(streamText);
            var tokens = lexer.Tokenize();

            throw new NotImplementedException("TextSIIParser.ParseStream not implemented");
        }
    }
}