using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using PrismLibrary.Sii.Parsing.Binary;
using PrismLibrary.Sii.Parsing.Binary.DataTypes;
using PrismLibrary.Sii.Parsing.Binary.Exceptions;

namespace PrismLibrary.Sii.Serializing.Binary
{
    public class BinarySIISerializer : ISIISerializer
    {
        private BinaryWriter writer;
        private Stream stream;
        private SIIFile file;

        internal BinarySIISerializer()
        {
        }
        
        public void WriteToStream(SIIFile file, Stream outputStream)
        {
            using var binaryWriter = new BinaryWriter(outputStream);
            writer = binaryWriter;
            stream = outputStream;
            this.file = file;

            var fileHeader = new FileHeader
            {
                Signature = new[] {'B', 'S', 'I', 'I'}
            };
            
            writer.WriteStructure(fileHeader);
            WriteUnitDeclarationHeaders();
            WriteUnits();
            writer.Write((byte) 0); // 0 at the eof
        }

        private void WriteUnitDeclarationHeaders()
        {
            List<UnitDeclaration> unitTypes = file.Units.Select(unit => unit.Type).Distinct().ToList();
            
            for (var i = 0; i < unitTypes.Count; i++)
            {
                UnitDeclaration unitType = unitTypes[i];

                if (unitType.Name.Length > 64)
                    throw new SiiBinaryFormatException("The unit declaration's name length is too long (> 64)");
                
                var header = new UnitDeclarationHeader
                {
                    Flag = i == 0 ? 2 : 0,
                    Format = 0,
                    Index1 = 1,
                    Index2 = unitType.Index,
                    NameLength = unitType.Name.Length
                };

                writer.WriteStructure(header);
                writer.WriteCharString(unitType.Name);

                WriteUnitDeclarationProperties(unitType);
            }

            writer.Write(0); // signifies the end of headers
        }

        private void WriteUnitDeclarationProperties(UnitDeclaration unitType)
        {
            foreach (PropertyDeclaration property in unitType.Properties)
            {
                if (!PropertyDeclarationHeader.TypeIndexByPropertyType.TryGetValue(property.Type, out int typeIndex))
                    throw new SiiBinaryFormatException($"The type index for property '{property.Type}' type is unknown.");
                
                var header = new PropertyDeclarationHeader
                {
                    Type = typeIndex,
                    NameLength = property.Name.Length
                };

                writer.WriteStructure(header);
                writer.WriteCharString(property.Name);

                if (property.Type == PropertyType.MaybeKeyValueArray)
                {
                    writer.Write(property.ValidValues.Count);

                    foreach (MaybeKeyValue kv in property.ValidValues)
                    {
                        var kvHeader = new MaybeKeyValueHeader
                        {
                            Value = kv.Value,
                            NameLength = kv.Name.Length
                        };

                        writer.WriteStructure(kvHeader);
                        writer.WriteCharString(kv.Name);
                    }
                }
            }
        }

        private void WriteUnits()
        {
            foreach (SiiUnit unit in file.Units)
            {
                writer.Write(unit.Type.Index);
                WriteUnitName(writer, unit.Name);
                PropertySerializer.SerializeUnitProperties(writer, unit);
            }

            writer.Write(0); // signifies end of units
        }

        internal static void WriteUnitName(BinaryWriter writer, string name)
        {
            if (name == null)
            {
                writer.Write((sbyte) 0);
                return;
            }

            if (name.StartsWith("_nameless."))
            {
                ulong id = 0;
                var hexStrings = name.Split('.').Skip(1).ToList();

                if (hexStrings.Count > 4)
                    throw new SiiBinaryFormatException("Nameless token has more than 4 hex strings");

                for (var i = 0; i < hexStrings.Count; i++)
                {
                    string hexString = hexStrings[i];
                    ushort value = ushort.Parse(hexString, NumberStyles.HexNumber);
                    id |= value;

                    if (i < hexStrings.Count - 1)
                        id <<= 16;
                }

                writer.Write((sbyte) -1); // signifies this unit name is tokenless (aka _nameless)
                writer.Write(id);
            }
            else
            {
                var tokens = name.Split('.').ToList();
                writer.Write((sbyte) tokens.Count);

                foreach (string token in tokens)
                {
                    writer.Write(EncodeToken(token));
                }
            }
        }

        internal static readonly char[] ValidTokenCharacters = "\00123456789abcdefghijklmnopqrstuvwxyz_".ToCharArray();

        internal static ulong EncodeToken(string token)
        {
            if (token.Length > 12)
                throw new SiiBinaryFormatException($"Token length > 12 (token = '{token}')");
            
            ulong encodedToken = 0;
            ulong multiplier = 1;

            for (var i = 0; i < token.Length; ++i)
            {
                byte tokenIndex = FindTokenIndex(token[i]);

                encodedToken += multiplier * tokenIndex;
                multiplier *= 38;
            }

            return encodedToken;
        }

        private static byte FindTokenIndex(in char ch)
        {
            for (byte i = 0; i < ValidTokenCharacters.Length; ++i)
            {
                if (ch == ValidTokenCharacters[i])
                    return i;
            }

            return 0;
        }
    }
}