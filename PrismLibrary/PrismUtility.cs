using System.Collections.Generic;
using System.Linq;

namespace PrismLibrary
{
    internal class PrismUtility
    {
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

                yield return new KeyValuePair<string, string>(key, value);
            }
        }
    }
}