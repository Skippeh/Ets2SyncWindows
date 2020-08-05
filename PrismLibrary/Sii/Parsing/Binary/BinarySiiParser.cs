// Wouldn't be possible without this as reference: https://github.com/davidsantos-br/ETS2Sync-Helper-4/blob/master/src/Ets2/Parser/SiiBinary.cpp

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using PrismLibrary.Sii.Parsing.Binary.DataTypes;
using PrismLibrary.Sii.Parsing.Binary.Exceptions;
using PrismLibrary.Sii.Serializing.Binary;

namespace PrismLibrary.Sii.Parsing.Binary
{
    public class BinarySIIParser : ISIIParser
    {
        private Dictionary<int, UnitDeclaration> unitDeclarations;

        private Stream stream;
        private BinaryReader reader;

        // dont allow other assemblies to construct this manually (use SiiParsing instead)
        internal BinarySIIParser()
        {
        }

        public SIIFile ParseStream(Stream inputStream)
        {
            unitDeclarations = new Dictionary<int, UnitDeclaration>();

            using var binaryReader = new BinaryReader(inputStream, Encoding.UTF8, true);
            reader = binaryReader;
            stream = inputStream;

            var header = reader.ReadStructure<FileHeader>();

            if (new string(header.Signature) != "BSII")
                throw new SiiBinaryFormatException($"The file format is not binary. Signature = {new string(header.Signature)}");

            ReadUnitDeclarationHeaders();
            var units = ReadUnits();

            if (stream.Position != stream.Length - 1)
                throw new NotImplementedException("Reached end of parsing but not the end of stream");

            stream = null;
            reader = null;

            var result = ParseFile(units);
            return result;
        }

        private static SIIFile ParseFile(List<UnitValue> units)
        {
            var result = new SIIFile();

            // First create and temporarily store SiiUnits in dictionary so we can reference them later when parsing unit properties
            var unitDictionary = new Dictionary<string, SiiUnit>();
            foreach (UnitValue unit in units)
            {
                unitDictionary.Add(unit.Name, new SiiUnit
                {
                    Name = unit.Name,
                    Type = unit.Type
                });
            }

            // Parse properties
            foreach (UnitValue unit in units)
            {
                var siiUnit = unitDictionary[unit.Name];
                result.Units.Add(siiUnit);

                siiUnit.Properties = unit.Values.Select(propertyValue =>
                {
                    object value;

                    SiiUnit FindUnit(string unitName)
                    {
                        if (unitName == null)
                            return null;

                        if (!unitDictionary.TryGetValue(unitName, out var result))
                        {
                            // Assume non existent unit names to be resource units from the game files.
                            var resourceUnit = new SiiResourceUnit
                            {
                                Name = unitName
                            };

                            unitDictionary.Add(unitName, resourceUnit);
                            return resourceUnit;
                        }

                        return result;
                    }

                    switch (propertyValue.Type)
                    {
                        case PropertyType.Unit:
                        {
                            value = FindUnit((string) propertyValue.Value);

                            break;
                        }
                        case PropertyType.UnitArray:
                        {
                            var list = new List<SiiUnit>();
                            value = list;

                            foreach (string unitName in ((List<object>) propertyValue.Value))
                            {
                                list.Add(FindUnit(unitName));
                            }

                            break;
                        }
                        default:
                        {
                            value = propertyValue.Value;
                            break;
                        }
                    }

                    return new SiiProperty
                    {
                        Name = propertyValue.Name,
                        Value = value,
                        Type = propertyValue.Type
                    };
                }).ToList();
            }

            return result;
        }

        private void ReadUnitDeclarationHeaders()
        {
            while (true)
            {
                var header = reader.ReadStructure<UnitDeclarationHeader>();

                if (header.Format != 0)
                {
                    // this is not a unit declaration header, rewind to the position of Format and return.
                    stream.Position -= 13;
                    return;
                }

                if (header.NameLength > 64)
                    throw new SiiBinaryFormatException("Unit declaration header name length too long (> 64)");

                string name = new string(reader.ReadChars(header.NameLength));
                var unitDeclaration = new UnitDeclaration
                {
                    Index = header.Index2,
                    Name = name
                };

                unitDeclarations.Add(unitDeclaration.Index, unitDeclaration);
                ReadUnitDeclarationProperties(unitDeclaration, header);
            }
        }

        private void ReadUnitDeclarationProperties(UnitDeclaration unitDeclaration, UnitDeclarationHeader unitHeader)
        {
            while (true)
            {
                var header = reader.ReadStructure<PropertyDeclarationHeader>();

                if (header.Type == 0)
                {
                    // this is not a property declaration, go back to the position before we read this structure
                    stream.Position -= Marshal.SizeOf<PropertyDeclarationHeader>();
                    return;
                }

                PropertyDeclaration propertyDeclaration = new PropertyDeclaration
                {
                    Name = new string(reader.ReadChars(header.NameLength))
                };
                
                if (header.Type >= PropertyDeclarationHeader.PropertyTypesByIndex.Length || PropertyDeclarationHeader.PropertyTypesByIndex[header.Type] == PropertyType.Invalid)
                {
                    throw new SiiBinaryFormatException($"Property declaration {unitDeclaration.Name}.{propertyDeclaration.Name} has an unknown type. Type = {header.Type}");
                }
                
                if (PropertyDeclarationHeader.PropertyTypesByIndex[header.Type] == PropertyType.MaybeKeyValueArray)
                {
                    List<MaybeKeyValue> types = new List<MaybeKeyValue>();
                    int count = reader.ReadInt32();

                    for (int i = 0; i < count; ++i)
                    {
                        var keyValueHeader = reader.ReadStructure<MaybeKeyValueHeader>();

                        string name = new string(reader.ReadChars(keyValueHeader.NameLength));
                        types.Add(new MaybeKeyValue
                        {
                            Name = name,
                            Value = keyValueHeader.Value
                        });
                    }

                    propertyDeclaration.ValidValues = types;
                }

                propertyDeclaration.Type = PropertyDeclarationHeader.PropertyTypesByIndex[header.Type];

                unitDeclaration.Properties.Add(propertyDeclaration);
            }
        }

        private List<UnitValue> ReadUnits()
        {
            var units = new List<UnitValue>();
            
            while (true)
            {
                int declarationIndex = reader.ReadInt32();

                if (declarationIndex == 0) // no more properties for this unit
                    break;

                if (!unitDeclarations.TryGetValue(declarationIndex, out UnitDeclaration unitDeclaration))
                    throw new SiiBinaryFormatException($"Unit uses unknown unit declaration index. Index = {declarationIndex}");

                string unitName = ReadUnitName(reader);
                var properties = PropertyParser.ParsePropertyDeclarations(unitDeclaration, reader);

                units.Add(new UnitValue
                {
                    Name = unitName,
                    Type = unitDeclaration,
                    Values = properties
                });
            }

            return units;
        }

        internal static string ReadUnitName(BinaryReader reader)
        {
            sbyte numTokens = reader.ReadSByte();

            if (numTokens == -1)
            {
                ulong id = reader.ReadUInt64();
                var a = (ushort) (id >> 48);
                var b = (ushort) (id >> 32);
                var c = (ushort) (id >> 16);
                var d = (ushort) id;

                var unitNameBuilder = new StringBuilder("_nameless.");

                if (a != 0)
                    unitNameBuilder.Append($"{a:x}.");

                if (b != 0)
                    unitNameBuilder.Append($"{b:x}.");

                if (c != 0)
                    unitNameBuilder.Append($"{c:x}.");

                unitNameBuilder.Append($"{d:x}");

                return unitNameBuilder.ToString();
            }

            if (numTokens < 1)
            {
                return null;
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < numTokens; ++i)
            {
                builder.Append(DecodeToken(reader.ReadUInt64()));

                if (i < numTokens - 1)
                    builder.Append('.');
            }

            return builder.ToString();
        }

        internal static string DecodeToken(ulong token)
        {
            ulong originalToken = token;
            StringBuilder builder = new StringBuilder(12, 12);

            while (token > 0)
            {
                byte charIndex = (byte) (token % 38);

                if (charIndex == 0 || charIndex >= BinarySIISerializer.ValidTokenCharacters.Length)
                    throw new SiiBinaryFormatException($"Invalid character in decoded token, Token = {originalToken}, charIndex = {charIndex}");

                char currentChar = BinarySIISerializer.ValidTokenCharacters[charIndex];
                builder.Append(currentChar);
                token /= 38;
            }

            return builder.ToString();
        }
    }
}