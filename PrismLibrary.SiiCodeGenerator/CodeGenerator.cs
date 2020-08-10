using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.VisualBasic.CompilerServices;
using NLog;
using PrismLibrary.Sii;
using PrismLibrary.Sii.Attributes;
using PrismLibrary.Sii.Parsing.Binary.DataTypes;

namespace PrismLibrary.SiiCodeGenerator
{
    public class CodeGenerator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private SIIFile file;
        private readonly Dictionary<PropertyDeclaration, string> cachedUnitTypeNames = new Dictionary<PropertyDeclaration, string>();

        public async Task<ICollection<(UnitDeclaration, string)>> GenerateClasses(SIIFile file, string namespaceName)
        {
            var result = new BlockingCollection<(UnitDeclaration, string)>();
            var tasks = new List<Task>();
            
            lock (cachedUnitTypeNames)
            {
                cachedUnitTypeNames.Clear();
            }

            this.file = file;

            foreach (UnitDeclaration unitDeclaration in file.Units.Select(unit => unit.Type).Distinct())
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        string classResult = await GenerateClassForUnit(unitDeclaration, namespaceName);
                        result.Add((unitDeclaration, classResult));
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, $"Failed to generate class for unit: {unitDeclaration.Name}: {ex.Message}");
                    }
                }));
            }

            await Task.WhenAll(tasks);
            return result.ToList();
        }

        private Task<string> GenerateClassForUnit(UnitDeclaration unitDeclaration, string namespaceName)
        {
            return Task.Run<string>(() =>
            {
                string className = FormatUtility.ConvertToCamelCase(unitDeclaration.Name);
                var workspace = new AdhocWorkspace();
                var generator = SyntaxGenerator.GetGenerator(workspace, LanguageNames.CSharp);

                SyntaxNode[] namespaces =
                {
                    generator.NamespaceImportDeclaration("System"),
                    generator.NamespaceImportDeclaration("System.Collections.Generic"),
                    generator.NamespaceImportDeclaration("PrismLibrary.Sii"),
                    generator.NamespaceImportDeclaration("PrismLibrary.Sii.Attributes"),
                    generator.NamespaceImportDeclaration("PrismLibrary.Sii.Parsing.Binary.DataTypes")
                };

                var memberNodes = new List<SyntaxNode>();

                foreach (var property in unitDeclaration.Properties)
                {
                    SyntaxNode[] attributeArguments =
                    {
                        generator.AttributeArgument(generator.LiteralExpression(property.Name)),
                        generator.AttributeArgument(generator.MemberAccessExpression(generator.IdentifierName(nameof(PropertyType)), property.Type.ToString()))
                    };
                    var unitAttributeNode = generator.Attribute(nameof(SiiPropertyAttribute), attributeArguments);

                    var propertyNode = generator.PropertyDeclaration(FormatUtility.ConvertToCamelCase(property.Name),
                        GetMemberTypeExpression(unitDeclaration, property, property.Type, generator, out bool isObjectType));
                    propertyNode = generator.AddAttributes(propertyNode, unitAttributeNode);

                    if (isObjectType)
                    {
                        // add comment
                        SyntaxTriviaList comment = SyntaxFactory.ParseLeadingTrivia(
                            @"/// <summary>
                            /// Warning: generic type. Could not get non-generic type from source.
                            /// </summary>" + "\n"
                        );

                        propertyNode = propertyNode.WithLeadingTrivia(comment);
                    }

                    memberNodes.Add(propertyNode);
                }

                var classDeclaration = generator.ClassDeclaration(className, null, Accessibility.Public, default, null, null, memberNodes);
                classDeclaration = generator.AddAttributes(
                    classDeclaration,
                    generator.Attribute(nameof(SiiUnitAttribute),
                        generator.AttributeArgument(generator.LiteralExpression(unitDeclaration.Name)))
                );

                var rootSyntaxNodes = new List<SyntaxNode>();
                rootSyntaxNodes.AddRange(namespaces);

                if (namespaceName == null)
                    rootSyntaxNodes.Add(classDeclaration);
                else
                    rootSyntaxNodes.Add(generator.NamespaceDeclaration(namespaceName, classDeclaration));

                return generator.CompilationUnit(rootSyntaxNodes).NormalizeWhitespace().ToFullString();
            });
        }

        private SyntaxNode GetMemberTypeExpression(UnitDeclaration unitDeclaration, PropertyDeclaration property, PropertyType propertyType, SyntaxGenerator generator, out bool isObjectType)
        {
            isObjectType = false;

            if (PropertyDeclarationHeader.PropertyArrayTypesByPropertyType.TryGetValue(propertyType, out PropertyType arrayPropertyType))
            {
                if (arrayPropertyType != PropertyType.MaybeKeyValueArray)
                {
                    var listTypeExpression = generator.GenericName("List", GetMemberTypeExpression(unitDeclaration, property, arrayPropertyType, generator, out isObjectType));
                    return listTypeExpression;
                }

                return generator.TypeExpression(SpecialType.System_Int32);
            }

            switch (propertyType)
            {
                case PropertyType.Boolean: return generator.TypeExpression(SpecialType.System_Boolean);
                case PropertyType.Float: return generator.TypeExpression(SpecialType.System_Single);
                case PropertyType.Int32: return generator.TypeExpression(SpecialType.System_Int32);
                case PropertyType.Int64: return generator.TypeExpression(SpecialType.System_Int64);
                case PropertyType.Token:
                case PropertyType.String: return generator.TypeExpression(SpecialType.System_String);
                case PropertyType.Uint16: return generator.TypeExpression(SpecialType.System_UInt16);
                case PropertyType.Uint32:
                case PropertyType.Uint32_2: return generator.TypeExpression(SpecialType.System_UInt32);
                case PropertyType.Uint64: return generator.TypeExpression(SpecialType.System_UInt64);
                case PropertyType.Unit:
                case PropertyType.Unit_2:
                case PropertyType.Unit_3:
                {
                    var unitTypeName = FindTypeNameOfUnitProperty(unitDeclaration, property, out isObjectType);
                    return SyntaxFactory.ParseTypeName(unitTypeName);
                }
                case PropertyType.FloatDual:
                {
                    return SyntaxFactory.ParseTypeName("Vector2");
                }
                case PropertyType.FloatTriple:
                {
                    return SyntaxFactory.ParseTypeName("Vector3");
                }
                case PropertyType.FloatQuad:
                {
                    return SyntaxFactory.ParseTypeName("Vector4");
                }
                case PropertyType.FloatTripleQuad:
                {
                    return SyntaxFactory.ParseTypeName("(Vector3, Vector4)");
                }
                case PropertyType.Int32Triple:
                {
                    return SyntaxFactory.ParseTypeName("Vector3Int");
                }
                case PropertyType.Uint32Triple:
                {
                    return SyntaxFactory.ParseTypeName("Vector3Uint");
                }
                default:
                {
                    isObjectType = true;
                    Logger.Warn($"Unknown TypeExpression for PropertyType: {propertyType}");
                    return generator.TypeExpression(SpecialType.System_Object);
                }
            }
        }

        private readonly object findTypeNameLock = new object();

        private string FindTypeNameOfUnitProperty(UnitDeclaration unitDeclaration, PropertyDeclaration propDeclaration, out bool isObjectType)
        {
            lock (findTypeNameLock)
            {
                isObjectType = false;

                if (propDeclaration.Type != PropertyType.Unit &&
                    propDeclaration.Type != PropertyType.Unit_2 &&
                    propDeclaration.Type != PropertyType.Unit_3 &&
                    propDeclaration.Type != PropertyType.UnitArray &&
                    propDeclaration.Type != PropertyType.UnitArray_2)
                {
                    throw new ArgumentException("Property declaration type needs to be a unit type or unit array type.", nameof(propDeclaration));
                }

                lock (cachedUnitTypeNames)
                {
                    if (cachedUnitTypeNames.TryGetValue(propDeclaration, out var cachedTypeName))
                        return cachedTypeName;
                }

                foreach (SiiUnit unit in file.Units)
                {
                    if (unit.Type != unitDeclaration)
                        continue;

                    var propertyValue = unit.Properties.First(prop => prop.Name == propDeclaration.Name);

                    if (propertyValue.Value != null)
                    {
                        string typeName;

                        if (propertyValue.Value is List<SiiUnit> objectList)
                        {
                            SiiUnit value = objectList.FirstOrDefault();

                            if (value == null)
                                continue;

                            if (value is SiiResourceUnit)
                                typeName = "string";
                            else
                                typeName = FormatUtility.ConvertToCamelCase(value.Type.Name);
                        }
                        else
                        {
                            if (propertyValue.Value is SiiResourceUnit)
                                typeName = "string";
                            else
                                typeName = FormatUtility.ConvertToCamelCase(((SiiUnit) propertyValue.Value).Type.Name);
                        }

                        lock (cachedUnitTypeNames)
                        {
                            cachedUnitTypeNames.Add(propDeclaration, typeName);
                        }

                        return typeName;
                    }
                }
                
                if (Program.CustomPropertyTypes.TryGetValue($"{unitDeclaration.Name}.{propDeclaration.Name}", out string propTypeName))
                {
                    Logger.Info($"Using custom property type '{propTypeName}' for {unitDeclaration.Name}.{propDeclaration.Name}");
                    return propTypeName;
                }

                isObjectType = true;
                Logger.Warn($"Could not find type for unit property: {unitDeclaration.Name}.{propDeclaration.Name}");
                string fallbackName = nameof(SiiUnit);
                cachedUnitTypeNames.Add(propDeclaration, fallbackName);
                return fallbackName;
            }
        }
    }
}