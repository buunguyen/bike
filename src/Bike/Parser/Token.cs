namespace Bike.Parser
{
    using System.Diagnostics;

    [DebuggerStepThrough]
    public class Token
    {
        public TokenType Type { get; private set; }
        public string Text { get; private set; }
        public SourceLocation Location { get; private set; }

        public Token(TokenType type, string text, SourceLocation location)
        {
            Type = type;
            Text = text;
            Location = location;
        }

        public bool Is(TokenType tokenType)
        {
            return Type == tokenType;
        }

        public bool IsNot(TokenType tokenType)
        {
            return !Is(tokenType);
        }

        public override string ToString()
        {
            return string.Format("Type: {0}; Text: {1}; Location: {2}",
                                 Type, Text, Location);
        }
    }
}
