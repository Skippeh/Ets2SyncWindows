using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PrismLibrary.Sii.Parsing.Binary.DataTypes
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PropertyDeclarationHeader
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable FieldCanBeMadeReadOnly.Global

        public int Type;
        public int NameLength;

        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore FieldCanBeMadeReadOnly.Global

        public static readonly PropertyType[] PropertyTypesByIndex =
        {
            0,
            /* 1 */ PropertyType.String,
            /* 2 */ PropertyType.StringArray,
            /* 3 */ PropertyType.Token,
            /* 4 */ PropertyType.TokenArray,
            /* 5 */ PropertyType.Float,
            /* 6 */ PropertyType.FloatArray,
            /* 7 */ PropertyType.FloatDual,
            0,
            /* 9 */ PropertyType.FloatTriple,
            0, 0, 0, 0, 0, 0, 0,
            /* 17 */ PropertyType.Uint32Triple,
            /* 18 */ PropertyType.Int32TripleArray,
            0, 0, 0, 0, 0,
            /* 24 */ PropertyType.FloatQuadArray,
            /* 25 */ PropertyType.FloatTripleQuad,
            /* 26 */ PropertyType.FloatTripleQuadArray,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            /* 37 */ PropertyType.Uint32,
            /* 38 */ PropertyType.Uint32Array,
            /* 39 */ PropertyType.Uint32,
            /* 40 */ PropertyType.Uint32Array,
            0, 0,
            /* 43 */ PropertyType.Uint16,
            /* 44 */ PropertyType.Uint16Array,
            0, 0, 0, 0,
            /* 49 */ PropertyType.Int64,
            0,
            /* 51 */ PropertyType.Uint64,
            /* 52 */ PropertyType.Int64Array,
            /* 53 */ PropertyType.Boolean,
            /* 54 */ PropertyType.BooleanArray,
            /* 55 */ PropertyType.MaybeKeyValueArray,
            0,
            /* 57 */ PropertyType.Unit,
            /* 58 */ PropertyType.UnitArray,
            /* 59 */ PropertyType.Unit,
            /* 60 */ PropertyType.UnitArray,
            /* 31 */ PropertyType.Unit
        };

        public static readonly PropertyType[] PropertyArrayTypesByPropertyType =
        {
            0,
            /* STRING = 1, */ 0,
            /* STRING_ARRAY, */ PropertyType.String,
            /* TOKEN, */ 0,
            /* TOKEN_ARRAY, */ PropertyType.Token,
            /* FLOAT, */ 0,
            /* FLOAT_ARRAY, */ PropertyType.Float,
            /* FLOAT_DUAL */ 0,
            /* FLOAT_TRIPLE, */ 0,
            /* FLOAT_QUAD, */ 0,
            /* UINT32_TRIPLE, */ 0,
            /* INT32_TRIPLE */ 0,
            /* INT32_TRIPLE_ARRAY, */ PropertyType.Int32Triple,
            /* FLOAT_QUAD_ARRAY, */ PropertyType.FloatQuad,
            /* FLOAT_TRIPLE_QUAD, */ 0,
            /* FLOAT_TRIPLE_QUAD_ARRAY */ PropertyType.FloatTripleQuad,
            /* INT32, */ 0,
            /* UINT32, */ 0,
            /* UINT32_ARRAY, */ PropertyType.Uint32,
            /* UINT16, */ 0,
            /* UINT16_ARRAY, */ PropertyType.Uint16,
            /* INT64, */ 0,
            /* UINT64, */ 0,
            /* INT64_ARRAY, */ PropertyType.Int64,
            /* BOOLEAN, */ 0,
            /* BOOLEAN_ARRAY, */ PropertyType.Boolean,
            /* MAYBE_KEY_VALUE_ARRAY */ PropertyType.MaybeKeyValueArray,
            /* UNIT, */ 0,
            /* UNIT_ARRAY */ PropertyType.Unit
        };
    }
}