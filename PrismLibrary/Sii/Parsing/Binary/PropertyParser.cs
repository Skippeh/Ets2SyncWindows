using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using PrismLibrary.Sii.Parsing.Binary.DataTypes;
using PrismLibrary.Sii.Parsing.Binary.Exceptions;

namespace PrismLibrary.Sii.Parsing.Binary
{
    internal static class PropertyParser
    {
        public static List<PropertyValue> ParsePropertyDeclarations(UnitDeclaration unitDeclaration, BinaryReader reader)
        {
            var properties = new List<PropertyValue>();
            
            foreach (PropertyDeclaration property in unitDeclaration.Properties)
            {
                properties.Add(ReadProperty(reader, property.Type, property.Name));
            }

            return properties;
        }

        private static PropertyValue ReadProperty(BinaryReader reader, PropertyType type, string propertyName, bool isArrayMember = false, uint arrayIndex = 0)
        {
            var name = propertyName;
            object value;

            switch (type)
            {
                case PropertyType.Uint16:
                case PropertyType.Uint32:
                case PropertyType.Uint64:
                case PropertyType.Int32:
                case PropertyType.Int64:
                case PropertyType.Boolean:
                case PropertyType.Uint32Triple:
                case PropertyType.Int32Triple:
                case PropertyType.Token:
                case PropertyType.Unit:
                case PropertyType.String:
                case PropertyType.Float:
                case PropertyType.FloatDual:
                case PropertyType.FloatTriple:
                case PropertyType.FloatQuad:
                case PropertyType.FloatTripleQuad:
                {
                    value = ReadValue(reader, type);
                    break;
                }
                case PropertyType.TokenArray:
                case PropertyType.Int32TripleArray:
                case PropertyType.Uint32Array:
                case PropertyType.Uint16Array:
                case PropertyType.Int64Array:
                case PropertyType.BooleanArray:
                case PropertyType.UnitArray:
                case PropertyType.StringArray:
                case PropertyType.FloatArray:
                case PropertyType.FloatQuadArray:
                case PropertyType.FloatTripleQuadArray:
                case PropertyType.MaybeKeyValueArray:
                {
                    var arrayPropertyType = PropertyDeclarationHeader.PropertyArrayTypesByPropertyType[(int) type];
                    uint count = reader.ReadUInt32();
                    List<object> values = new List<object>();

                    if (arrayPropertyType == PropertyType.Invalid)
                        throw new SiiBinaryFormatException($"Unknown value property array type = {type}");

                    for (uint i = 0; i < count; ++i)
                    {
                        values.Add(ReadProperty(reader, arrayPropertyType, null, true, i).Value);
                    }

                    value = values;
                    break;
                }
                default: throw new NotImplementedException($"Unknown property type = {type}");
            }

            return new PropertyValue
            {
                Name = name,
                Value = value,
                Type = type
            };
        }

        private static object ReadValue(BinaryReader reader, PropertyType type)
        {
            switch (type)
            {
                case PropertyType.Uint16: return reader.ReadInt16();
                case PropertyType.Int32: return reader.ReadInt32();
                case PropertyType.Uint32: return reader.ReadUInt32();
                case PropertyType.Int64: return reader.ReadUInt64();
                case PropertyType.Uint64: return reader.ReadUInt64();
                case PropertyType.Boolean: return reader.ReadBoolean();
                case PropertyType.Float: return reader.ReadSingle();
                case PropertyType.String:
                {
                    uint length = reader.ReadUInt32();
                    char[] chars = reader.ReadChars((int) length);
                    string outString = new string(chars);
                    return outString;
                }
                case PropertyType.Token:
                {
                    return BinarySIIParser.DecodeToken(reader.ReadUInt64());
                }
                case PropertyType.Unit: return BinarySIIParser.ReadUnitName(reader);
                case PropertyType.FloatDual: return (reader.ReadSingle(), reader.ReadSingle());
                case PropertyType.FloatTriple: return (reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                case PropertyType.Uint32Triple: return (reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32());
                case PropertyType.Int32Triple: return (reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                case PropertyType.FloatQuad: return (reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                case PropertyType.FloatTripleQuad:
                {
                    var triple = ((float, float, float)) ReadValue(reader, PropertyType.FloatTriple);
                    byte[] unknown = reader.ReadBytes(4);
                    var quad = ((float, float, float, float)) ReadValue(reader, PropertyType.FloatQuad);

                    Console.WriteLine("FloatTripleQuad:");
                    Console.WriteLine($"Values: ({triple.Item1}, {triple.Item2}, {triple.Item3}) {quad.Item1}, {quad.Item2}, {quad.Item3}, {quad.Item4}");
                    Console.WriteLine($"Unknown bytes: 0x{unknown[0]:X}, 0x{unknown[1]:X}, 0x{unknown[2]:X}");
                    Console.WriteLine();
                    
                    return (triple, quad);
                }
                case PropertyType.MaybeKeyValueArray:
                {
                    break;
                }
            }

            throw new NotImplementedException($"Unknown type = {type}");
        }
    }
}