using System.IO;

namespace PrismLibrary.Sii.Parsing
{
    public interface ISIIParser
    {
        SIIFile ParseStream(Stream inputStream);
    }
}