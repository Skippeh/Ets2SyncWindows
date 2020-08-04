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
        // ReSharper disable InconsistentNaming
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
        FloatTripleQuad, // Not a quad of triples, but a triple followed by four bytes (garbage) followed by a quad
        FloatTripleQuadArray,
        Int32,
        Uint32,
        Uint32Array,
        Uint32_2,
        Uint32Array_2,
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
        Unit_2,
        UnitArray_2,
        Unit_3
        // ReSharper restore InconsistentNaming
    }
}