using System;
using System.IO;

namespace PrismLibrary.Sii.Serializing
{
    public static class SiiSerializing
    {
        public static void SerializeSiiFile<T>(SIIFile file, Stream outputStream) where T : ISIISerializer
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            if (outputStream == null)
                throw new ArgumentNullException(nameof(outputStream));

            if (!outputStream.CanWrite)
                throw new ArgumentException("Output stream is not writable.", nameof(outputStream));
            
            var serializer = (T) Activator.CreateInstance(typeof(T), true);
            serializer.WriteToStream(file, outputStream);
        }
    }
}