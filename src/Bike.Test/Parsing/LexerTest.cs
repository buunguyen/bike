namespace Bike.Test
{
    using System;
    using System.Text;
    using Parser;
    using NUnit.Framework;

    [TestFixture]
    public class LexerTest : BaseTest
    {
        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void NullSource()
        {
            new Lexer(null);
        }

        [Test]
        public void EmptySource()
        {
            var token = new Lexer("  ").NextToken();
            Assert.True(token.Type == TokenType.Eof);
        }

        [Test]
        public void Comment()
        {
            var token = new Lexer("# This is a comment").NextToken();
            Assert.True(token.Type == TokenType.Eof);
        }

        [Test]
        public void MultilineComment()
        {
            var token = new Lexer(@"#! This is a comment
                                       in multiple lines !#").NextToken();
            Assert.True(token.Type == TokenType.Eof);
        }

        [Test]
        public void AnotherMultilineComment()
        {
            var token = new Lexer(@"#!
                                        This is a comment
                                        in multiple lines 
                                    !#").NextToken();
            Assert.True(token.Type == TokenType.Eof);
        }

        [Test]
        public void SimpleAssignments()
        {
            const string source = @"var name = ""John"",
                                        age = 10,
                                        salary;
                                    salary = 70.55";
            const string expected = @"k$var i$name = 'John', 
                                        i$age = n$10, 
                                        i$salary; 
                                    i$salary = n$70.55<EOF>";
            Match(expected, source);
        }

        [Test]
        public void StringLiterals()
        {
            string source = "'this is a string'";
            var token = new Lexer(source).NextToken();
            Assert.AreEqual(TokenType.String, token.Type);
            Assert.AreEqual("this is a string", token.Text);

            source = "\"this is a string\"";
            token = new Lexer(source).NextToken();
            Assert.AreEqual(TokenType.String, token.Type);
            Assert.AreEqual("this is a string", token.Text);

            source = "\"this \n is \t a \r string \'\"";
            token = new Lexer(source).NextToken();
            Assert.AreEqual(TokenType.String, token.Type);
            Assert.AreEqual("this \n is \t a \r string \'", token.Text);

            source = "'this is a string"; // unfinished quote
            try
            {
                new Lexer(source).NextToken();
                Assert.Fail();
            } 
            catch (ParseException){}
        }

        [Test]
        public void Escapes()
        {
			// TODO: \n fails in Mac OSX's Mono
            const string source = "'\\'', '\\\"', '\\\\', '\\b', '\\t', '\\n', '\\f', '\\r'";
            const string expected = "'\'', '\"', '\\', '\b', '\t', '\n', '\f', '\r'<EOF>";
            Match(expected, source);
        }

        [Test]
        public void EscapesWithTail()
        {
            const string source = "'\\'bike', '\\tbike'";
            const string expected = "'\'bike', '\tbike'<EOF>";
            Match(expected, source);
        }

        [Test]
        public void Regex()
        {
            const string source = "'(?<=<img)\\\\s*(?i)'";
            const string expected = "'(?<=<img)\\s*(?i)'<EOF>";
            Match(expected, source);
        }

        [Test]
        public void NumberLiterals()
        {
            const string source = "12, 13.4, 0.5";
            const string expected = "n$12, n$13.4, n$0.5<EOF>";
            Match(expected, source);
        }

        [Test]
        public void ArrayLiterals()
        {
            const string source = "[1, '2', 1.5]";
            const string expected = "[n$1, '2', n$1.5]<EOF>";
            Match(expected, source);
        }

        [Test]
        public void ArrayRanges()
        {
            const string source = "[1->10]";
            const string expected = "[n$1->n$10]<EOF>";
            Match(expected, source);
        }

        [Test]
        public void If()
        {
            const string source = @"if (a && b || (c || (true && 'false' || 0.0)))";
            const string expected = "k$if ( i$a && i$b || ( i$c || (k$true && 'false' || n$0.0)))<EOF>";
            Match(expected, source);
        }

        [Test]
        public void Declaration()
        {
            const string source = @"v1!='a'";
            const string expected = "i$v1 != 'a'<EOF>";
            Match(expected, source);
        }

        [Test]
        public void Conditional()
        {
            const string source = @"return a ? b : c ? (d) : e";
            const string expected = "k$return i$a ? i$b : i$c ? (i$d) : i$e<EOF>";
            Match(expected, source);
        }

        [Test]
        public void IfElse()
        {
            const string source = @"if (a) {   
                                        body;
                                    } else if (b && c) 
                                        body
                                    else
                                        body";
            const string expected = @"k$if(i$a){ 
                                        i$body;
                                      } k$else k$if (i$b && i$c) 
                                        i$body
                                      k$else
                                        i$body<EOF>";
            Match(expected, source);
        }

        [Test]
        public void While()
        {
            const string source = @"while (a && b) c = 'a';";
            const string expected = "k$while(i$a && i$b) i$c = 'a';<EOF>";
            Match(expected, source);
        }

        [Test]
        public void ForIn()
        {
            const string source = @"for (var i in arr) { body; }";
            const string expected = "k$for (k$var i$i k$in i$arr) { i$body; }<EOF>";
            Match(expected, source);
        }

        [Test]
        public void TryRescueFinally()
        {
            const string source = @"try {} rescue e {} finally {}";
            const string expected = "k$try {} k$rescue i$e {} k$finally {}<EOF>";
            Match(expected, source);
        }

        [Test]
        public void Throw()
        {
            const string source = @"try {throw 'error'} rescue e {print(e)}";
            const string expected = "k$try {k$throw 'error'} k$rescue i$e {i$print(i$e)}<EOF>";
            Match(expected, source);
        }

        [Test]
        public void Cond()
        {
            const string source = @"switch (e) {case a: break; case b: break; default: break;}";
            const string expected = "k$switch (i$e) {k$case i$a: k$break; k$case i$b: k$break; k$default: k$break;}<EOF>";
            Match(expected, source);
        }

        [Test]
        public void FunctionDeclaration()
        {
            const string source = @"func(a, b) {c = func(){return a}; return a + c();}";
            const string expected = "k$func(i$a, i$b) {i$c = k$func(){k$return i$a}; k$return i$a + i$c();}<EOF>";
            Match(expected, source);
        }

        [Test]
        public void FunctionInvocation()
        {
            string source = @"person.curry (another()) ((func() {})())";
            string expected = "i$person d$. i$curry(i$another()) ((k$func() {})())<EOF>";
            Match(expected, source);

            source = @"func(true, func(status) {print(status);})";
            expected = @"k$func(k$true, k$func(i$status) {i$print(i$status);})<EOF>";
            Match(expected, source);
        }

        [Test]
        public void PrimitiveInvocation()
        {
            string source = @"1.5.to_s";
            string expected = "n$1.5 d$. i$to_s<EOF>";
            Match(expected, source);

            source = @"'abc'.to_i";
            expected = "'abc' d$. i$to_i<EOF>";
            Match(expected, source);
        }

        [Test]
        public void New()
        {
            const string source = @"var person = new Namespace.Person(1, '1')";
            const string expected = "k$var i$person = k$new i$Namespace d$. i$Person(n$1, '1')<EOF>";
            Match(expected, source);
        }

        [Test]
        public void References()
        {
            const string source = @"load 'library.bk'
                                    load 'Lib.dll'";
            const string expected = @"k$load 'library.bk' 
                                      k$load 'Lib.dll'<EOF>";
            Match(expected, source);
        }
       

        [Test]
        public void ObjectLiterals()
        {
            const string source = @"var person = {
                                        name: 'John', 
                                        age: tmpAge, 
                                        salary: 70.5,
                                        go: func(){this.age;}
                                    };";
            const string expected = @"k$var i$person = {  
                                        i$name: 'John',  
                                        i$age: i$tmpAge, 
                                        i$salary: n$70.5, 
                                        i$go: k$func(){k$this d$. i$age;} 
                                      };<EOF>";
            Match(expected, source);
        }

        private static void Match(string expected, string source)
        {
            var actual = new StringBuilder();
            var lexer = new Lexer(source);
            var token = lexer.NextToken();
            while (token.Type != TokenType.Eof)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                        actual.Append("n$").Append(token.Text);
                        break;
                    case TokenType.String:
                        actual.Append("'").Append(token.Text).Append("'");
                        break;
                    case TokenType.Identifier:
                        actual.Append("i$").Append(token.Text);
                        break;
                    case TokenType.Dot:
                        actual.Append("d$").Append(token.Text);
                        break;
                    default:
                        if (token.Type.IsKeyword())
                            actual.Append("k$").Append(token.Text);
                        else 
                            actual.Append(token.Text);
                        break;
                }
                token = lexer.NextToken();
            }
            actual.Append(token.Text);
            Assert.AreEqual(expected.Replace(Environment.NewLine, string.Empty).Replace(" ", string.Empty), 
                            actual.ToString());
        }
    }
}