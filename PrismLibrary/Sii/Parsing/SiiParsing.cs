using System;
using System.IO;

namespace PrismLibrary.Sii.Parsing
{
    public static class SiiParsing
    {
        public static SIIFile ParseStream<T>(Stream stream) where T : ISIIParser
        {
            var parser = (T) Activator.CreateInstance(typeof(T), true);
            return parser.ParseStream(stream);
        }
    }
}