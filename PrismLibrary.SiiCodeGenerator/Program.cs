using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using NLog;
using NLog.Config;
using NLog.Targets;
using PrismLibrary.Sii;
using PrismLibrary.Sii.Parsing;
using PrismLibrary.Sii.Parsing.Binary;
using PrismLibrary.Sii.Parsing.Binary.DataTypes;

namespace PrismLibrary.SiiCodeGenerator
{
    internal static class Program
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private static Task<int> Main(string[] args)
        {
            return Parser.Default.ParseArguments<LaunchArguments>(args).MapResult(MainWithArgs, _ => Task.FromResult(1));
        }

        private static Task<int> MainWithArgs(LaunchArguments args)
        {
            // Initialize NLog console logging
            var logConfig = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget("LogConsole");
            logConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, consoleTarget);
            LogManager.Configuration = logConfig;

            
            return Task.Run<int>(async () =>
            {
                try
                {
                    SIIFile siiFile;
                    
                    using (var fileStream = File.OpenRead(args.InputFilePath))
                    {
                        var bytes = PrismEncryption.DecryptAndDecompressFile(fileStream);
                        using var memoryStream = new MemoryStream(bytes, false);
                        siiFile = SiiParsing.ParseStream<BinarySIIParser>(memoryStream);
                    }

                    var codeGenerator = new CodeGenerator();
                    var classResults = await codeGenerator.GenerateClasses(siiFile, args.CodeNamespace);

                    if (!Directory.Exists(args.OutputDirPath))
                        Directory.CreateDirectory(args.OutputDirPath);

                    var tasks = new List<Task>();
                    
                    foreach ((UnitDeclaration, string) classResult in classResults)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            string filePath = Path.Combine(args.OutputDirPath, $"{FormatUtility.ConvertToCamelCase(classResult.Item1.Name)}.generated.cs");

                            using var fileStream = File.CreateText(filePath);
                            await fileStream.WriteAsync(classResult.Item2);
                        }));
                    }

                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    return 1;
                }
                finally
                {
                    LogManager.Shutdown();
                }

                return 0;
            });
        }
    }

    internal class LaunchArguments
    {
        [Option('i', "input", HelpText = "The filepath to the input .sii file to generate code from.", Required = true)]
        public string InputFilePath { get; set; }
        
        [Option('o', "output", HelpText = "The output directory path to put the generated classes in.", Required = true)]
        public string OutputDirPath { get; set; }

        [Option('n', "namespace", HelpText = "The namespace to put the generated classes in.")]
        public string CodeNamespace { get; set; }
    }
}