using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PrismLibrary.Sii.Parsing.Binary;
using PrismLibrary.Sii.Parsing.Binary.DataTypes;
using PrismLibrary.Sii.Parsing.Binary.Exceptions;

namespace PrismLibrary.Sii.Serializing.Binary
{
    internal static class PropertySerializer
    {
        public static void SerializeUnitProperties(BinaryWriter writer, SiiUnit unit)
        {
            foreach (SiiProperty property in unit.Properties)
            {
                WriteProperty(writer, property.Type, property.Value);
            }
        }

        private static void WriteProperty(BinaryWriter writer, PropertyType propertyType, object propertyValue)
        {
            switch (propertyType)
            {
                case PropertyType.Uint16:
                case PropertyType.Uint32:
                case PropertyType.Uint32_2:
                case PropertyType.Uint64:
                case PropertyType.Int32:
                case PropertyType.Int64:
                case PropertyType.Boolean:
                case PropertyType.Uint32Triple:
                case PropertyType.Int32Triple:
                case PropertyType.Token:
                case PropertyType.Unit:
                case PropertyType.Unit_2:
                case PropertyType.Unit_3:
                case PropertyType.String:
                case PropertyType.Float:
                case PropertyType.FloatDual:
                case PropertyType.FloatTriple:
                case PropertyType.FloatQuad:
                case PropertyType.FloatTripleQuad:
                {
                    WriteValue(writer, propertyType, propertyValue);
                    break;
                }
                case PropertyType.TokenArray:
                case PropertyType.Int32TripleArray:
                case PropertyType.Uint32Array:
                case PropertyType.Uint32Array_2:
                case PropertyType.Uint16Array:
                case PropertyType.Int64Array:
                case PropertyType.BooleanArray:
                case PropertyType.UnitArray:
                case PropertyType.UnitArray_2:
                case PropertyType.StringArray:
                case PropertyType.FloatArray:
                case PropertyType.FloatQuadArray:
                case PropertyType.FloatTripleQuadArray:
                case PropertyType.MaybeKeyValueArray:
                {
                    if (!PropertyDeclarationHeader.PropertyArrayTypesByPropertyType.TryGetValue(propertyType, out var arrayPropertyType))
                        throw new SiiBinaryFormatException($"There is no known property type for array type '{propertyType}'");

                    uint count;

                    if (propertyValue != null)
                    {
                        if (propertyValue is ICollection)
                        {
                            count = (uint) ((ICollection) propertyValue).Count;
                        }
                        else if (propertyValue.GetType().IsArray)
                        {
                            count = (uint) ((Array) propertyValue).Length;
                        }
                        else
                        {
                            throw new SiiBinaryFormatException("Property type is array but value is not a collection or array.");
                        }

                        if (count > 0 && propertyType == PropertyType.MaybeKeyValueArray)
                            throw new NotImplementedException($"Unimplemented case: MaybeKeyValueArray Count > 0 (count = {count})");
                    }
                    else
                    {
                        count = 0;
                    }

                    writer.Write(count);

                    if (count > 0)
                    {
                        foreach (object value in ((IEnumerable) propertyValue))
                        {
                            WriteProperty(writer, arrayPropertyType, value);
                        }
                    }

                    break;
                }
                default: throw new NotImplementedException($"Unknown property type = {propertyType}");
            }
        }

        private static void WriteValue(BinaryWriter writer, PropertyType type, object value)
        {
            switch (type)
            {
                case PropertyType.Uint16:
                {
                    writer.Write((ushort) value);
                    break;
                }
                case PropertyType.Int32:
                {
                    writer.Write((int) value);
                    break;
                }
                case PropertyType.Uint32:
                case PropertyType.Uint32_2:
                {
                    writer.Write((uint) value);
                    break;
                }
                case PropertyType.Int64:
                {
                    writer.Write((long) value);
                    break;
                }
                case PropertyType.Uint64:
                {
                    writer.Write((ulong) value);
                    break;
                }
                case PropertyType.Boolean:
                {
                    writer.Write((bool) value);
                    break;
                }
                case PropertyType.Float:
                {
                    writer.Write((float) value);
                    break;
                }
                case PropertyType.String:
                {
                    writer.WriteCharString((string) value, writeLength: true);
                    break;
                }
                case PropertyType.Token:
                {
                    writer.Write(BinarySIISerializer.EncodeToken((string) value));
                    break;
                }
                case PropertyType.Unit:
                case PropertyType.Unit_2:
                case PropertyType.Unit_3:
                {
                    BinarySIISerializer.WriteUnitName(writer, ((SiiUnit) value)?.Name);
                    break;
                }
                case PropertyType.FloatDual:
                {
                    var tuple = ((float, float)) value;
                    writer.Write(tuple.Item1);
                    writer.Write(tuple.Item2);
                    break;
                }
                case PropertyType.FloatTriple:
                {
                    var tuple = ((float, float, float)) value;
                    writer.Write(tuple.Item1);
                    writer.Write(tuple.Item2);
                    writer.Write(tuple.Item3);
                    break;
                }
                case PropertyType.Uint32Triple:
                {
                    var tuple = ((uint, uint, uint)) value;
                    writer.Write(tuple.Item1);
                    writer.Write(tuple.Item2);
                    writer.Write(tuple.Item3);
                    break;
                }
                case PropertyType.Int32Triple:
                {
                    var tuple = ((int, int, int)) value;
                    writer.Write(tuple.Item1);
                    writer.Write(tuple.Item2);
                    writer.Write(tuple.Item3);
                    break;
                }
                case PropertyType.FloatQuad:
                {
                    var tuple = ((float, float, float, float)) value;
                    writer.Write(tuple.Item1);
                    writer.Write(tuple.Item2);
                    writer.Write(tuple.Item3);
                    writer.Write(tuple.Item4);
                    break;
                }
                case PropertyType.FloatTripleQuad:
                {
                    var tripleQuad = (((float, float, float), (float, float, float, float))) value;

                    writer.Write(tripleQuad.Item1.Item1);
                    writer.Write(tripleQuad.Item1.Item2);
                    writer.Write(tripleQuad.Item1.Item3);
                    writer.Write(uint.MaxValue); // write 4 "garbage" bytes
                    writer.Write(tripleQuad.Item2.Item1);
                    writer.Write(tripleQuad.Item2.Item2);
                    writer.Write(tripleQuad.Item2.Item3);
                    writer.Write(tripleQuad.Item2.Item4);
                    break;
                }
                case PropertyType.MaybeKeyValueArray:
                {
                    throw new NotImplementedException("MaybeKeyValueArray impl missing");
                }
                default: throw new NotImplementedException($"Unknown property type = {type}");
            }
        }
    }
}