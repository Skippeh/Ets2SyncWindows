using System.Collections.Generic;

namespace PrismLibrary.Sii.Parsing.Binary.DataTypes
{
    public class PropertyDeclaration
    {
        public PropertyType Type;
        public string Name;
        public List<MaybeKeyValue> ValidValues;
    }
    
    public enum PropertyType
    {
        Invalid,
        String,
        StringArray,
        Token,
        TokenArray,
        Float,
        FloatArray,
        FloatDual,
        FloatTriple,
        FloatQuad,
        Uint32Triple,
        Int32Triple,
        Int32TripleArray,
        FloatQuadArray,
        FloatTripleQuad, // Not a quad of triples, but a triple followed by four bytes (?) followed by a quad
        FloatTripleQuadArray,
        Int32,
        Uint32,
        Uint32Array,
        Uint16,
        Uint16Array,
        Int64,
        Uint64,
        Int64Array,
        Boolean,
        BooleanArray,
        MaybeKeyValueArray,
        Unit,
        UnitArray,
    }
}