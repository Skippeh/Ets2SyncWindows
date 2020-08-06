namespace PrismLibrary.Sii.Parsing.Text
{
    public class Token
    {
        public TokenType Type;
        public object Value;

        public Token(TokenType type, object value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case TokenType.OpeningBrace:
                case TokenType.ClosingBrace:
                case TokenType.OpeningBracket:
                case TokenType.ClosingBracket:
                case TokenType.OpeningParenthesis:
                case TokenType.ClosingParenthesis:
                case TokenType.Colon:
                case TokenType.Comma:
                case TokenType.SemiColon:
                case TokenType.Boolean:
                case TokenType.Null:
                case TokenType.RootUnitDeclaration:
                {
                    return Type.ToString();
                }
                default: return $"[{Type}] {Value}";
            }
        }
    }
    
    public enum TokenType
    {
        /// <summary>
        /// The root unit declaration at the top of the file
        /// </summary>
        RootUnitDeclaration,
        /// <summary>
        /// Opening brace, aka {
        /// </summary>
        OpeningBrace,
        /// <summary>
        /// Closing brace, aka }
        /// </summary>
        ClosingBrace,
        /// <summary>
        /// Opening bracket, aka [
        /// </summary>
        OpeningBracket,
        /// <summary>
        /// Closing bracket, aka ]
        /// </summary>
        ClosingBracket,
        /// <summary>
        /// Opening parenthesis, aka (
        /// </summary>
        OpeningParenthesis,
        /// <summary>
        /// Closing parenthesis, aka )
        /// </summary>
        ClosingParenthesis,
        /// <summary>
        /// An identifier that matches another unit in the file or a resource unit from the game.
        /// </summary>
        Identifier,
        /// <summary>
        /// A number. Can be int, ulong, double, etc.
        /// </summary>
        Number,
        /// <summary>
        /// Comma, aka ,
        /// </summary>
        Comma,
        /// <summary>
        /// Colon, aka :
        /// </summary>
        Colon,
        /// <summary>
        /// Semi-colon, aka ;
        /// </summary>
        SemiColon,
        /// <summary>
        /// A character string.
        /// </summary>
        String,
        /// <summary>
        /// The literal "true" or "false".
        /// </summary>
        Boolean,
        /// <summary>
        /// The literal "null".
        /// </summary>
        Null
    }
}