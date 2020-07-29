using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PrismLibrary.Exceptions;

namespace PrismLibrary
{
    public class PrismConfig
    {
        public Dictionary<string, string> KeyValues { get; private set; }
        
        /// <summary>
        /// Gets the filepath to the config file, if it was loaded. Otherwise null.
        /// </summary>
        public string FilePath { get; private set; }

        private PrismConfig()
        {
            KeyValues = new Dictionary<string, string>();
        }

        public static PrismConfig CreateConfig()
        {
            return new PrismConfig();
        }

        public static PrismConfig LoadConfig(FileStream fileStream)
        {
            PrismConfig result = CreateConfig();
            result.FilePath = fileStream.Name;
            string[] fileLines = FileUtility.ReadAllLines(fileStream).ToArray();

            for (var i = 0; i < fileLines.Length; i++)
            {
                string line = fileLines[i];
                string trimmedLine = line.Trim();
                string[] parts = trimmedLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (trimmedLine.StartsWith("#") || string.IsNullOrEmpty(trimmedLine)) // skip comment and empty lines
                    continue;
                
                string key;
                string value;

                if (parts[0] != "uset")
                {
                    throw new ConfigFormatException($"Found unexpected instruction '{parts[0]}' on line {i} in config file.")
                    {
                        Line = i,
                        Instruction = parts[0]
                    };
                }

                key = parts[1];
                value = string.Join(' ', parts.Skip(2)).Trim('"');

                result.KeyValues[key] = value;
            }

            return result;
        }

        public static async Task<bool> SaveConfig(PrismConfig config, string filePath, bool backupOriginalFile = true)
        {
            if (backupOriginalFile && File.Exists(filePath))
            {
                var extension = Path.GetExtension(filePath);
                string backupFilePath = Path.ChangeExtension(filePath, $"{extension}.backup");

                if (!File.Exists(backupFilePath))
                {
                    File.Copy(filePath, backupFilePath, false);
                }
            }

            await Task.Run(() =>
            {
                using var streamWriter = File.CreateText(config.FilePath);
                streamWriter.WriteLine("# prism3d variable config data");
                streamWriter.WriteLine();

                foreach (var kv in config.KeyValues)
                {
                    streamWriter.WriteLine($"uset {kv.Key} \"{kv.Value}\"");
                }
            });

            return false;
        }

        public string this[string key]
        {
            get => KeyValues.GetValueOrDefault(key, null);
            set => KeyValues[key] = value;
        }

        public async Task<bool> Save(bool backupOriginalFile = true)
        {
            return await SaveConfig(this, FilePath, backupOriginalFile);
        }
    }
}