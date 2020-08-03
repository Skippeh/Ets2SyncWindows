using System;
using System.IO;

namespace PrismLibrary.Sii.Parsing.Binary.Exceptions
{
    public class SiiBinaryFormatException : FileFormatException
    {
        public SiiBinaryFormatException(string message) : base(message)
        {
        }

        public SiiBinaryFormatException()
        {
        }
    }
}