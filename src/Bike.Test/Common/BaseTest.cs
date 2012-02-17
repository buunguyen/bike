namespace Bike.Test
{
    using System;
    using Parser;

    public abstract class BaseTest
    {
        protected void ParseAndWalk(string source)
        {
            Console.WriteLine();
            Console.WriteLine(">> Tree for: " + source);
            var parser = new Parser(new Lexer(source));
            var ast = parser.Parse();
            ast.Accept(new PrintNodeWalker());
        }
    }
}
