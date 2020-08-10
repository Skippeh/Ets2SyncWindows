using PrismLibrary.Sii.Parsing.Binary.DataTypes;

namespace PrismLibrary.Sii
{
    public class SiiProperty
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public PropertyType Type { get; set; }

        public override string ToString()
        {
            return $"{Name} = {Value}";
        }
    }
}