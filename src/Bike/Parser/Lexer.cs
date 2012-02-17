namespace Bike.Parser
{
    using System;
    using System.Text;

    internal class Lexer
    {
        private readonly char[] sourceChars;
        private readonly int totalChars;
        private int currentLineIndex = 1;
        private int currentColumnIndex = 1;
        private SourceLocation currentLocation;
        private int currentIndex;
        private int currentChar;
        private readonly string filePath;
        private const int Eof = -1;

        public Lexer(string source, string filePath = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            this.filePath = filePath ?? "<no file>";
            sourceChars = source.ToCharArray();
            totalChars = sourceChars.Length;
            currentChar = totalChars == 0
                ? Eof
                : sourceChars[0];
        }

        private void Next()
        {
            currentIndex++;
            currentColumnIndex++;
            if (currentIndex >= totalChars)
            {
                currentChar = Eof;
            }
            else
            {
                currentChar = sourceChars[currentIndex];
                var previousChar = currentIndex >= 1 ? sourceChars[currentIndex - 1] : Eof;
                if (currentChar == '\r' || 
                    (currentChar == '\n' && (previousChar == Eof || previousChar != '\r')))
                {
                    currentLineIndex++;
                    currentColumnIndex = 1;
                }
            }
        }

        private int Peek(int ahead = 1)
        {
            int index = ahead + currentIndex;
            return index >= totalChars
                       ? Eof
                       : sourceChars[index];
        }

        private void Match(char expectedChar)
        {
            if (currentChar == expectedChar)
                Next();
            else
                throw Error("Expect '" + expectedChar + "', found '" + (char)currentChar + "'");
        }

        public Token NextToken()
        {
            while (currentChar != Eof)
            {
                currentLocation = new SourceLocation(filePath, currentLineIndex, currentColumnIndex);
                switch ((char)currentChar)
                {
                    case '\r':
                    case '\n':
                    case ' ':
                    case '\t':
                        Whitespace();
                        continue;
                    case '#':
                        Comment();
                        continue;
                    case '=':
                        return Equal();
                    case '<':
                        return LessThan();
                    case '>':
                        return GreaterThan();
                    case '*':
                        return Star();
                    case '/':
                        return Slash();
                    case '%':
                        return Percent();
                    case '!':
                        return Bang();
                    case '+':
                        return Plus();
                    case '-':
                        return Minus();
                    case '|':
                        return Pipe();
                    case '&':
                        return Ampersand();
                    case ',':
                        Match(',');
                        return Token(TokenType.Comma, ",");
                    case ';':
                        Match(';');
                        return Token(TokenType.SemiColon, ";");
                    case '.':
                        Match('.');
                        return Token(TokenType.Dot, ".");
                    case '?':
                        Match('?');
                        return Token(TokenType.Question, "?");
                    case ':':
                        Match(':');
                        return Token(TokenType.Colon, ":");
                    case '[':
                        Match('[');
                        return Token(TokenType.LeftBracket, "[");
                    case ']':
                        Match(']');
                        return Token(TokenType.RightBracket, "]");
                    case '(':
                        Match('(');
                        return Token(TokenType.LeftParen, "(");
                    case ')':
                        Match(')');
                        return Token(TokenType.RightParen, ")");
                    case '{':
                        Match('{');
                        return Token(TokenType.LeftBrace, "{");
                    case '}':
                        Match('}');
                        return Token(TokenType.RightBrace, "}");
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        return Number();
                    case '\'':
                    case '"':
                        return String();
                    default:
                        if (Char.IsLetter((char) currentChar) ||
                            currentChar == '_' ||   
                            currentChar == '$')
                            return Identifier();
                        throw Error("Invalid character: " + currentChar);
                }
            }
            return Token(TokenType.Eof, "<EOF>");
        }

        /// <summary>
        /// &&, &&=
        /// </summary>
        private Token Ampersand()
        {
            Match('&');
            Match('&');
            if (currentChar == '=')
            {
                Match('=');
                return Token(TokenType.OpAssign, "&&=");
            }
            return Token(TokenType.LogicalAnd, "&&");
        }

        private Token Pipe()
        {
            Match('|');
            Match('|');
            if (currentChar == '=')
            {
                Match('=');
                return Token(TokenType.OpAssign, "||=");
            }
            return Token(TokenType.LogicalOr, "||");
        }

        private Token Minus()
        {
            Match('-');
            switch (currentChar)
            {
                case '-':
                    Match('-');
                    return Token(TokenType.DoubleMinus, "--");
                case '=': 
                    Match('=');
                    return Token(TokenType.OpAssign, "-=");
                case '>':
                    Match('>');
                    return Token(TokenType.Derive, "->");
                default:
                    return Token(TokenType.Minus, "-");
            }
        }

        private Token Plus()
        {
            Match('+');
            switch (currentChar)
            {
                case '+':
                    Match('+');
                    return Token(TokenType.DoublePlus, "++");
                case '=':
                    Match('=');
                    return Token(TokenType.OpAssign, "+=");
                default:
                    return Token(TokenType.Plus, "+");
            }
        }

        private Token Bang()
        {
            Match('!');
            if (currentChar == '=')
            {
                Match('=');
                return Token(TokenType.NotEqual, "!=");
            }
            return Token(TokenType.Not, "!");
        }

        private Token Percent()
        {
            Match('%');
            if (currentChar == '=')
            {
                Match('=');
                return Token(TokenType.OpAssign, "%=");
            }
            return Token(TokenType.Modulus, "%");
        }

        private Token Slash()
        {
            Match('/');
            if (currentChar == '=')
            {
                Match('=');
                return Token(TokenType.OpAssign, "/=");
            }
            return Token(TokenType.Divide, "/");
        }

        private Token Star()
        {
            Match('*');
            if (currentChar == '=')
            {
                Match('=');
                return Token(TokenType.OpAssign, "*=");
            }
            return Token(TokenType.Multiply, "*");
        }

        private Token LessThan()
        {
            Match('<');
            if (currentChar == '=')
            {
                Match('=');
                return Token(TokenType.LessThanOrEqual, "<=");
            }
            return Token(TokenType.LessThan, "<");
        }

        private Token GreaterThan()
        {
            Match('>');
            if (currentChar == '=')
            {
                Match('=');
                return Token(TokenType.GreaterThanOrEqual, ">=");
            }
            return Token(TokenType.GreaterThan, "<");
        }

        private Token Equal()
        {
            Match('=');
            if (currentChar == '=')
            {
                Match('=');
                return Token(TokenType.Equal, "==");
            }
            return Token(TokenType.OpAssign, "=");
        }

        private Token String()
        {
            var openChar = (char)currentChar;
            var buf = new StringBuilder();
            while (true)
            {
                Next();
                if (currentChar == openChar)
                {
                    Match(openChar);
                    break;
                }
                if (currentChar == Eof)
                    throw Error("Unexpected end of token");
                if (currentChar == '\\')
                {
                    char escape = Escape();
                    buf.Append(escape);
                }
                else
                    buf.Append((char)currentChar);
            } 
            return Token(TokenType.String, buf.ToString());
        }

        private char Escape()
        {
            Match('\\');
            switch (currentChar)
            {
                case 'b':
                    return '\b';
                case 't':
                    return '\t';
                case 'n':
                    return '\n';
                case 'f':
                    return '\f';
                case 'r':
                    return '\r';
                case '\'':
                    return '\'';
                case '\"':
                    return '"';
                case '\\':
                    return '\\';
                default:
                    throw Error("Invalid escape character " + currentChar);
            }
        }

        private Token Number()
        {
            var buf = new StringBuilder();
            do
            { 
                buf.Append((char) currentChar);
                Next();
            } while (Char.IsDigit((char) currentChar));
            
            if (currentChar != '.' || 
                !Char.IsDigit((char)Peek()) // function invocation
                )
                return Token(TokenType.Number, buf.ToString());
            
            // so, it's a dot and follow by a number, read decimal
            do
            { 
                buf.Append((char) currentChar);
                Next();
            } while (Char.IsDigit((char) currentChar));
            return Token(TokenType.Number, buf.ToString()); 
        }

        private Token Identifier()
        {
            var buf = new StringBuilder();
            do
            {
                buf.Append((char) currentChar);
                Next();
            } while (Char.IsLetter((char) currentChar) ||
                     Char.IsDigit((char) currentChar) ||
                     currentChar == '_' ||
                     currentChar == '$');
            var identifier = buf.ToString();
            var keyword = identifier.KeywordFromString();
            return Token(keyword == TokenType.None
                             ? TokenType.Identifier
                             : keyword, identifier);
        }

        private void Whitespace()
        {
            while (currentChar == ' ' ||
                   currentChar == '\t' ||
                   currentChar == '\r' ||
                   currentChar == '\n')
            {
                Next();
            }
        }

        private void Comment()
        {
            if (Peek() == '!')
            {
                Match('#');
                Match('!');
                do
                {
                    Next();
                } while (currentChar != '!' || Peek() != '#');
                Match('!');
                Match('#');
                return;
            }
            do
            {
                Next();
            } while (currentChar != '\r' && currentChar != '\n' && currentChar != Eof);
        }

        private Exception Error(string msg)
        {
            return new ParseException(msg, currentLocation);
        }

        private Token Token(TokenType type, string txt)
        {
            return new Token(type, txt, currentLocation);
        }
    }
}