using System.IO;

namespace PrismLibrary.Sii.Serializing
{
    public interface ISIISerializer
    {
        void WriteToStream(SIIFile file, Stream outputStream);
    }
}