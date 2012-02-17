namespace Bike.Parser
{
    using System;
    using System.Text;

    public class ParseException : Exception
    {
        internal readonly SourceLocation Location;

        public ParseException(string msg, Token token)
            : this(msg, token.Location) {}

        public ParseException(string msg, SourceLocation location)
            : base(msg)
        {
            Location = location;
        }

        public override string ToString()
        {
            return new StringBuilder().Append(GetType().Name)
                .Append(": ")
                .Append(Message)
                .AppendLine()
                .AppendFormat("   in {0}:line {1}", Location.FilePath, Location.Line)
                .AppendLine().ToString();
        }
    }
}
