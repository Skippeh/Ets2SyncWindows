using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PrismLibrary
{
    internal class PrismUtility
    {
        private static readonly Regex UnicodeRegex = new Regex(
            @"(?>\\x(?>[a-f0-9]){2}){2}",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled
        );
        
        public static IEnumerable<KeyValuePair<string, string>> ParseSiiTextFile(string fileContents)
        {
            foreach (string line in fileContents.Replace("\r", "").Split("\n"))
            {
                string trimmedLine = line.Trim();

                if (!trimmedLine.Contains(':'))
                    continue;
                
                // The file format is a bit more advanced than a simple key:value format, but it works well enough for our purpose.
                string[] strKv = trimmedLine.Split(':');
                string key = strKv[0];
                string value = string.Join(" ", strKv.Skip(1)).Trim().Trim('"');

                if (value.Contains("\\"))
                {
                    string originalValue = value;
                    
                    Match match;
                    while ((match = UnicodeRegex.Matches(value).FirstOrDefault()) != null)
                    {
                        var splitIndex = match.Value.LastIndexOf("\\");
                        var hexStringA = match.Value.Substring(0, splitIndex).Substring(2);
                        var hexStringB = match.Value.Substring(splitIndex).Substring(2);

                        byte byteA = byte.Parse(hexStringA, NumberStyles.HexNumber);
                        byte byteB = byte.Parse(hexStringB, NumberStyles.HexNumber);
                        string unicodeChar = Encoding.UTF8.GetString(new[] {byteA, byteB});

                        value = value.Remove(match.Index, match.Length);
                        value = value.Insert(match.Index, unicodeChar);
                    }

                    value = value.Replace(@"\\", @"\");
                }

                yield return new KeyValuePair<string, string>(key, value);
            }
        }
    }
}