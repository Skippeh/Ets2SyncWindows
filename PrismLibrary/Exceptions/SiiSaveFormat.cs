using System.IO;

namespace PrismLibrary.Exceptions
{
    /// <summary>
    /// Indicates a failure when trying to read an encrypted sii file.
    /// </summary>
    public class SiiFormatException : IOException
    {
        public enum ParseError
        {
            InvalidSize,
            InvalidSignature,
            NotEncrypted
        }

        public ParseError ErrorType { get; set; }

        public SiiFormatException(ParseError errorType) : base($"Could not read the SII file due to unexpected file format. ({errorType})")
        {
            ErrorType = errorType;
        }
    }
}