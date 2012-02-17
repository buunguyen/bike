namespace Bike.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerStepThrough]
    internal class BacktrackParser
    {
        private readonly Lexer lexer;
        private readonly Stack<int> markers;
        private readonly List<Token> buffer;
        private int currentTokenIndex;

        public BacktrackParser(Lexer lexer)
        {
            this.lexer = lexer;
            markers = new Stack<int>();
            buffer = new List<Token>();
            Sync(1);
        }

        private void Sync(int noOfTokens)
        {
            int fillNumber = (currentTokenIndex + noOfTokens) - buffer.Count;
            if (fillNumber > 0) Fill(fillNumber);
        }

        private void Fill(int noOfTokens)
        {
            for (int i = 0; i < noOfTokens; i++)
            {
                buffer.Add(lexer.NextToken());
            }
        }

        protected void Consume()
        {
            currentTokenIndex++;
            // Hit end of buffer while not speculating
            if (currentTokenIndex == buffer.Count && !IsSpeculating())
            {
                currentTokenIndex = 0;
                buffer.Clear();
            }
            Sync(1);
        }

        protected Token Next(int i = 1)
        {
            Sync(i);
            return buffer[currentTokenIndex + i - 1];
        }

        protected void Match(TokenType type)
        {
            var token = Next();
            if (token.Is(type))
                Consume();
            else
                throw Error(string.Format("Expect ('{0}'), found ('{1}')",
                    type, token.Type), token);
        }

        protected void Match(TokenType type, string text)
        {
            var token = Next();
            if (token.Is(type) && token.Text == text)
                Consume();
            else
                throw Error(string.Format("Expect ('{0}', '{1}'), found ('{2}', '{3}')", 
                    type, text,
                    token.Type, token.Text), token);
        }

        protected void Mark()
        {
            markers.Push(currentTokenIndex);
        }

        protected void Release(bool success)
        {
            var marker = markers.Pop();
            if (!success)
                currentTokenIndex = marker;
        }

        private bool IsSpeculating()
        {
            return markers.Count > 0;
        }

        protected bool Speculate<T>(Func<T> func, out T result, 
            Func<T, bool> condition = null) where T : class
        {
            Mark();
            try
            {
                result = func();
                if (condition != null && !condition(result))
                    throw new ParseException(string.Empty, Next());
                Release(true);
                return true;
            }
            catch (ParseException)
            {
                result = null;
                Release(false);
                return false;
            }
        }

        protected Exception Error(string msg, Token token)
        {
            return new ParseException(msg, token);
        }
    }
}