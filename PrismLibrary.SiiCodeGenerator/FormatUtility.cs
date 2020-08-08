using System.Text;

namespace PrismLibrary.SiiCodeGenerator
{
    public static class FormatUtility
    {
        private static readonly char[] CamelCaseSeparators =
        {
            '_'
        };
        
        public static string ConvertToCamelCase(string inputText)
        {
            string[] split = inputText.Split(CamelCaseSeparators);
            var builder = new StringBuilder(inputText.Length);

            for (var i = 0; i < split.Length; ++i)
            {
                string str = split[i];

                if (char.IsLower(str[0]))
                {
                    str = char.ToUpperInvariant(str[0]) + str.Substring(1);
                }

                builder.Append(str);
            }

            return builder.ToString();
        }
    }
}