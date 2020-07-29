using System;

namespace PrismLibrary.Exceptions
{
    public class ConfigFormatException : Exception
    {
        public int Line;
        public string Instruction;

        public ConfigFormatException() { }
        public ConfigFormatException(string message) : base(message) { }
    }
}