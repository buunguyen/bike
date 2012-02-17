namespace Bike.Parser
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class TokenTypeHelper
    {
        private static readonly Dictionary<string, TokenType> Keywords =
            new Dictionary<string, TokenType>
            {
                {"load", TokenType.Load},
                {"var", TokenType.Var},
                {"func", TokenType.Func},
                {"while", TokenType.While},
                {"for", TokenType.For},
                {"in", TokenType.In},
                {"next", TokenType.Next},
                {"break", TokenType.Break},
                {"return", TokenType.Return},
                {"if", TokenType.If},
                {"else", TokenType.Else}, 
                {"switch", TokenType.Switch},
                {"case", TokenType.Case},
                {"default", TokenType.Default},
                {"try", TokenType.Try},
                {"rescue", TokenType.Rescue},
                {"finally", TokenType.Finally},
                {"throw", TokenType.Throw},
                {"new", TokenType.New},
                {"exec", TokenType.Exec},
                {"is", TokenType.Is},
                {"this", TokenType.This},
                {"true", TokenType.True},
                {"false", TokenType.False},
                {"null", TokenType.Null}
            };

        public static bool IsKeyword(this TokenType tokenType)
        {
            return Keywords.Values.Contains(tokenType);
        }

        public static TokenType KeywordFromString(this string str)
        {
            return Keywords.ContainsKey(str)
                       ? Keywords[str]
                       : TokenType.None;
        }
    }   

    public enum TokenType
    {
        None = 0,
        Comma,
        SemiColon,
        Dot,
        Colon,
        Grave, // `
        LeftBracket,  // [
        RightBracket, // ]
        LeftParen,
        RightParen,
        LeftBrace,  // {
        RightBrace, // }
        Eof,
        Escape,
        OpAssign,
        LogicalAnd,
        LogicalOr,
        DoubleMinus,
        Minus,
        DoublePlus,
        Plus,
        NotEqual,
        Derive, // ->
        Not,
        LessThanOrEqual,
        LessThan,
        GreaterThanOrEqual,
        GreaterThan,
        Modulus,
        Divide,
        Multiply,
        Question,
        Equal,
        Number,
        String,
        Identifier,

        // Keywords
        Load,
        Var,
        Func,
        While,
        For,
        In,
        Next,
        Break,
        Return,
        If,
        Else,
        Switch,
        Try,
        Rescue,
        Throw,
        Finally,
        New,
        Is,
        Case,
        Default,
        This,
        True,
        False,
        Null,
        Exec
    }
}
