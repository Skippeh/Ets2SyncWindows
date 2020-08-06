using System.IO;

namespace PrismLibrary.Sii.Parsing
{
    public class SiiParsingException : FileFormatException
    {
        public SiiParsingException(string message) : base(message)
        {
        }

        public SiiParsingException()
        {
        }
    }
}