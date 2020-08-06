using System;

namespace PrismLibrary.Sii.Parsing.Text.Exceptions
{
    public class SiiTextFormatException : SiiParsingException
    {
        public SiiTextFormatException(string message) : base(message)
        {
        }

        public SiiTextFormatException()
        {
        }
    }
}