namespace Bike.Test
{
    using NUnit.Framework;

    [TestFixture]
    public class ParserTest : BaseTest
    {
        [Test]
        public void Empty()
        {
            ParseAndWalk("");
            ParseAndWalk("    ");
            ParseAndWalk("# This is a comment");
            ParseAndWalk("# This is # a comment");
            ParseAndWalk(@"#! This is a multiple line
                              comment
                           !# 'and a string';");
            ParseAndWalk(@"#! if (!filePath.ends('.cs')) return; !#");
        }

        [Test]
        public void Load()
        {
            ParseAndWalk(@"load 'io.bk';");
            ParseAndWalk(@"var code = 'bike.bk';
                           load code;");
        }

        [Test]
        public void Globals()
        {
            ParseAndWalk(
                @"(func() {
                     $PATH = 'a';
                     (func(){$PATH = 'b';})();
                  })();");
        }

        [Test]
        public void Unary()
        {
            ParseAndWalk("+1;");
            ParseAndWalk("-1;");
            ParseAndWalk("++1;");
            ParseAndWalk("--1;");
            ParseAndWalk("!1;");
            ParseAndWalk("1++;");
            ParseAndWalk("1--;");
            ParseAndWalk("!1 + -2 - +3 * ++4 / --5;");
            ParseAndWalk("!!!!!!b;");
            ParseAndWalk("!++--++p;");
        }

        [Test]
        public void Maths()
        {
            ParseAndWalk("true % 2;");
            ParseAndWalk("1 + 2 * 3 / 4 % 5;");
            ParseAndWalk("23 * 14.4 + 2.1 / 1 - 3.99 % 9.5;");
            ParseAndWalk("(1 + 2) * (3 + 4) / (5 - 6) % (7 - 8);");
            ParseAndWalk("(a);");
            ParseAndWalk("((((a))));");
            ParseAndWalk("((((a + b))) - (c / d * (e + ((f % g)/h))));");
        }

        [Test]
        public void Declarations()
        {
            ParseAndWalk("var a;");
            ParseAndWalk("var a, b, c;");
            ParseAndWalk("var a = 1;");
            ParseAndWalk("a = null;");
            ParseAndWalk("a = true;");
            ParseAndWalk("a = false;");
            ParseAndWalk("a = 'abc';");
            ParseAndWalk("var a = 1 + 2, b = 2, c = b;");
            ParseAndWalk("a = 11.15; b = true; c = b;");
            ParseAndWalk(@"var a = 1 + 2 + 0.5;");
        }

        [Test]
        public void Objects()
        {
            ParseAndWalk(@"person = {};");
            ParseAndWalk(@"person = {name: 10};");
            ParseAndWalk(@"person = { name: 'John', 
                                      age: 10,
                                      to_s: func() {return this.name + ' ' + this.age;}
                                    };");
            ParseAndWalk(@"name = person.name;");
            ParseAndWalk(@"person.to_s();");
            ParseAndWalk(@"person = { __name: 'John', $value: 10 };");
        }

        [Test]
        public void Arrays()
        {
            ParseAndWalk("var ar = [1, [2], 3, 4, 5];");
            ParseAndWalk("ar[0] = 1;");
            ParseAndWalk("ar[1][0] = 2;");
            ParseAndWalk("ar[b()][c[d[1]]];");
            ParseAndWalk("arr = Core.init([1, 2, 3]);");
            ParseAndWalk("var ar = [1->5];");
            ParseAndWalk("var ar = ['a'->'z'];");
            ParseAndWalk("var ar = ['A'->'Z'];");
            ParseAndWalk("var ar = [a->b];");
        }
        
        [Test]
        public void Functions()
        {
            ParseAndWalk(@"func f(){}");
            ParseAndWalk(@"f();");
            ParseAndWalk(@"f()()();");
            ParseAndWalk(@"f(1, a, 'b');");
            ParseAndWalk(@"f(1, a, null);");
            ParseAndWalk(@"(func(){})(a, b);");
            ParseAndWalk(@"(func f(){})(1, '2');");
            ParseAndWalk(@"func f(){return a;}");
            ParseAndWalk(@"o.f();");
            ParseAndWalk(@"o.f();");
            ParseAndWalk(@"o.f(p);");
            ParseAndWalk(@"o.p.f(p);");
            ParseAndWalk(@"f(1)(1);");
            ParseAndWalk(@"f[1](1);");
            ParseAndWalk(@"f[1](1).p;");
            ParseAndWalk(@"o.f(p)[1](p);");
            ParseAndWalk(@"Console.WriteLine('123');");
            ParseAndWalk(@"f(*a, *b);");
            ParseAndWalk(@"func f(a, *b){};");
        }

        [Test]
        public void Exec()
        {
            ParseAndWalk(@"var a = exec 'a';");
            ParseAndWalk(@"var a = exec a.b().a;");
        }

        [Test]
        public void DeepInvoke()
        {
            ParseAndWalk(@"Console.WriteLine();");
            ParseAndWalk(@"Console.Out.WriteLine();");
            ParseAndWalk(@"a.ToString();");
            ParseAndWalk(@"(func(){})().a.b.c();");
            ParseAndWalk(@"[1, 2].a.b.c;");
            ParseAndWalk(@"a.b.c.d.e.f;");
            ParseAndWalk(@"a.b.c.d().e.f;");
            ParseAndWalk(@"a.b.c.d.e.f();");
            ParseAndWalk(@"a.b.c().d().e.f().g[1].h;");
            ParseAndWalk(@"a().b.c().d().e.f().g[1].h;");
            ParseAndWalk(@"Console.WriteLine();");
            ParseAndWalk(@"{p: 1}.a();");;
        }

        [Test]
        public void Conditional()
        {
            ParseAndWalk("a = b ? c : d;");
            ParseAndWalk("a = b ? c : d ? e : f;");
            ParseAndWalk("a < b;");
            ParseAndWalk("a < b && b > c;");
            ParseAndWalk("a.m < b ? b > c : d >= e;");
            ParseAndWalk("if(a);");
            ParseAndWalk("if(a)f1();");
            ParseAndWalk("if(a) {f1();f2();}");
            ParseAndWalk("if(a)f1();else f2(); f3();");
            ParseAndWalk("if(a)f1();else{f2();f3();}");
            ParseAndWalk("if(a)f1();else if(b){f2();} else f3();");
        }

        [Test]
        public void While()
        {
            ParseAndWalk("while(a)f();");
            ParseAndWalk("while(a){f();}");
            ParseAndWalk("while(a++);");
            ParseAndWalk("while(a && b) while (c);;");
            ParseAndWalk("while(a++) {if(b) break; else next;}");
        }

        [Test]
        public void ForIn()
        {
            ParseAndWalk("for(var m in c);");
            ParseAndWalk("for(m in c)f();");
            ParseAndWalk("for(m in c){f1();f2();}");
            ParseAndWalk("for(m in c){if(a)break;if(b)next;f();}");
            ParseAndWalk("for(m1 in c1) for(m2 in c2) f();");
            ParseAndWalk("for(m1 in c1) { for(m2 in c2) f(); }");
        }

        [Test]
        public void Switch()
        {
            ParseAndWalk("switch (exp) {}");
            ParseAndWalk(@"switch (exp) {
                                case a: 
                                    f1();
                                    break;
                                case b: 
                                    f1();
                                    break;
                           }");
            ParseAndWalk(@"switch (exp) {
                                case a: 
                                case b: 
                                    f1();
                                    break;
                                default: 
                                    f1();
                                    break;
                           }");
            ParseAndWalk(@"switch (exp) {
                                case a:             
                                    f1();
                                case b: 
                                    f2();
                                    break;
                                default: {
                                    f1();
                                    break;
                                }
                           }");
        }

        [Test]
        public void Constructors()
        {
            ParseAndWalk(@"Person();");
            ParseAndWalk(@"System.List<System.List<System.String>>();");
            ParseAndWalk(@"System.Dictionary<System.String, System.Int32>();");
            ParseAndWalk(@"System.Dictionary<System.List<System.List<System.Int32>>, 
                                        System.List<System.List<System.String>>>();");
            ParseAndWalk(@"String('123');");
            ParseAndWalk(@"Person(name, 'Coder', 20);");
            ParseAndWalk(@"Person().DoSomething(1, 2);");
        }

        [Test]
        public void Is()
        {
            ParseAndWalk(@"a is Person<System.String>;");
            ParseAndWalk(@"a is System.Collections.Generic.List<System.String>;");
        }

        [Test]
        public void Exception()
        {
            ParseAndWalk(@"try {
	                           throw 'a';
                           } rescue {
                               print('error');
                           }");
            ParseAndWalk(@"try {
	                           throw 'a';
                           } rescue err {
                               print(err);
                           }");
            ParseAndWalk(@"try {
	                           throw 'a';
                           } finally {
                               print('done');
                           }");
            ParseAndWalk(@"try {
	                           throw 'a';
                           } rescue err {
                               print(err);
                           } finally {
                               print('done');
                           }");
        }
    }
}
