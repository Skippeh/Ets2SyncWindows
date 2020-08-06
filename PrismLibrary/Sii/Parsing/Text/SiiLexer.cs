using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PrismLibrary.Sii.Parsing.Text.Exceptions;

namespace PrismLibrary.Sii.Parsing.Text
{
    public class SiiLexer
    {
        private delegate Token Lexeme(in char ch);
        
        public readonly string Text;

        private int position;
        private int line;
        private int column;
        private bool EndOfText => position >= Text.Length - 1;

        internal static readonly Dictionary<char, TokenType> Punctuators = new Dictionary<char, TokenType>
        {
            {':', TokenType.Colon},
            {';', TokenType.SemiColon},
            {'{', TokenType.OpeningBrace},
            {'}', TokenType.ClosingBrace},
            {'[', TokenType.OpeningBracket},
            {']', TokenType.ClosingBracket},
            {'(', TokenType.OpeningParenthesis},
            {')', TokenType.ClosingParenthesis}
        };

        internal readonly Dictionary<string, TokenType> Literals = new Dictionary<string, TokenType>
        {
            {"true", TokenType.Boolean},
            {"false", TokenType.Boolean},
            {"null", TokenType.Null}
        };

        private readonly Lexeme[] lexemes;

        public SiiLexer(in string siiText)
        {
            Text = siiText ?? throw new ArgumentNullException(nameof(siiText));

            lexemes = new Lexeme[]
            {
                TryLexRoot,
                TryLexPunctuator,
                TryLexQuotedString,
                TryLexNumber,
                
                // always last
                TryLexLiteral,
                TryLexIdentifier
            };
        }

        public List<Token> Tokenize()
        {
            var result = new List<Token>();
            position = 0;
            line = 0;
            column = 0;
            
            while (!EndOfText)
            {
                SkipWhiteSpace();
                SkipComments();

                Token token = null;
                char peekChar = PeekChar();
                
                foreach (Lexeme lexeme in lexemes)
                {
                    token = lexeme(peekChar);

                    if (token != null)
                    {
                        result.Add(token);
                        break;
                    }
                }

                if (token != null)
                    continue;

                throw new SiiTextFormatException($"Unexpected character '{peekChar}' ({((byte) peekChar):X}) at {GetLineColumnString()}.");
            }

            return result;
        }

        private char ReadChar()
        {
            if (position >= Text.Length - 1)
                throw new IndexOutOfRangeException("Tried to read past end of text");
            
            char readChar = Text[position++];

            if (readChar == '\n')
            {
                column = 0;
                ++line;
            }
            else
            {
                ++column;
            }
            
            return readChar;
        }

        private string ReadChars(int numChars)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < numChars; ++i)
            {
                builder.Append(ReadChar());
            }
            
            return builder.ToString();
        }

        private char PeekChar(int offset = 0)
        {
            if (position + offset >= Text.Length - 1 || position + offset < 0)
                return '\0';

            return Text[position + offset];
        }

        private bool PeekEquals(in string chars, int offset = 0)
        {
            for (int i = 0; i < chars.Length; ++i)
            {
                if (PeekChar(offset + i) != chars[i])
                    return false;
            }

            return true;
        }

        private string ReadUntil(char ch, bool appendChar = false, bool includeWhiteSpace = false)
        {
            StringBuilder builder = new StringBuilder();

            char currentChar;
            while ((currentChar = ReadChar()) != ch)
            {
                if (includeWhiteSpace || !char.IsWhiteSpace(currentChar))
                    builder.Append(currentChar);
            }

            if (appendChar)
                builder.Append(ch); // append target char
            
            return builder.ToString();
        }

        private void SkipWhiteSpace() => SkipWhile(char.IsWhiteSpace);
        
        private void SkipUntil(char ch) => SkipWhile(ch2 => ch != ch2);

        private string ReadWhile(Predicate<char> predicate)
        {
            StringBuilder builder = new StringBuilder();

            while (!EndOfText && predicate(PeekChar()))
            {
                builder.Append(ReadChar());
            }
            
            return builder.ToString();
        }

        private void SkipWhile(Predicate<char> predicate)
        {
            while (!EndOfText && predicate(PeekChar()))
                ReadChar();
        }

        private void SkipChars(int numChars)
        {
            position += numChars;
        }

        private void SkipComments()
        {
            SkipLineComment();
            SkipBlockComment();
        }

        private void SkipBlockComment()
        {
            if (PeekEquals("/*"))
            {
                SkipChars(2);
                SkipWhile(ch => !PeekEquals("*/"));
            }
        }

        private void SkipLineComment()
        {
            if (PeekEquals("//"))
            {
                SkipWhile(ch => ch != '\n');
                SkipChars(1); // skip \n
            }
        }

        private string GetLineColumnString()
        {
            return $"line {line + 1}, column {column + 1}";
        }

        private Token TryLexRoot(in char ch)
        {
            if (PeekEquals("unit") && position == 0)
            {
                SkipChars(4);
                return new Token(TokenType.RootUnitDeclaration, "unit");
            }

            return null;
        }

        private Token TryLexPunctuator(in char ch)
        {
            if (Punctuators.TryGetValue(ch, out var tokenType))
            {
                SkipChars(1);
                return new Token(tokenType, ch);
            }

            return null;
        }

        private Token TryLexQuotedString(in char ch)
        {
            if (ch == '"')
            {
                SkipChars(1); // skip "
                string str = ReadWhile(ch2 => ch2 != '"');
                SkipChars(1); // skip "
                return new Token(TokenType.String, str);
            }

            return null;
        }

        private static readonly char[] ValidNumberChars = "0123456789".ToCharArray();
        private static readonly char[] ValidNumberPunctuators = ".".ToCharArray();
        private static readonly char[] ValidHexChars = "&abcdef".ToCharArray();
        
        private Token TryLexNumber(in char ch)
        {
            var builder = new StringBuilder();
            
            bool isHex = ch == '&';

            int offset = 0;
            while (true)
            {
                char currentChar = char.ToLowerInvariant(PeekChar(offset++));

                if (ValidNumberChars.Contains(currentChar) || (offset > 0 && ValidNumberPunctuators.Contains(currentChar)) || isHex && ValidHexChars.Contains(currentChar))
                {
                    builder.Append(currentChar);
                }
                else
                {
                    break;
                }
            }

            if (builder.Length > 0)
            {
                string numberString = builder.ToString();
                object value;

                if (numberString.Contains('&') || numberString.Contains('.'))
                {
                    if (numberString.StartsWith('&'))
                    {
                        byte[] bytes = BitConverter.GetBytes(uint.Parse(numberString.Substring(1), NumberStyles.HexNumber));
                        value = BitConverter.ToSingle(bytes);
                    }
                    else
                        value = float.Parse(numberString, CultureInfo.InvariantCulture);
                }
                else
                {
                    value = ulong.Parse(numberString, CultureInfo.InvariantCulture);
                }

                SkipChars(numberString.Length);
                return new Token(TokenType.Number, value);
            }

            return null;
        }

        private Token TryLexLiteral(in char ch)
        {
            foreach (var kv in Literals)
            {
                if (PeekEquals(kv.Key))
                {
                    SkipChars(kv.Key.Length);

                    object value;

                    switch (kv.Value)
                    {
                        case TokenType.Boolean:
                        {
                            value = bool.Parse(kv.Key);
                            break;
                        }
                        case TokenType.Null:
                        {
                            value = null;
                            break;
                        }
                        default: throw new NotImplementedException($"Unhandled literal token type: {kv.Value}");
                    }

                    return new Token(kv.Value, value);
                }
            }

            return null;
        }

        private Token TryLexIdentifier(in char ch)
        {
            string identifier = ReadWhile(ch2 => !Punctuators.ContainsKey(ch2) && !char.IsWhiteSpace(ch2));
            return new Token(TokenType.Identifier, identifier);
        }
    }
}