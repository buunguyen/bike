namespace Bike.Test
{
    using NUnit.Framework;

    [TestFixture]
    public class LanguageTest : BaseInterpreterTest
    {
        [Test]
        public void NotExist()
        {
             InterpretFail("NotDefinedError", @"a = 1; var a;");
        }

        [Test]
        public void Declarations()
        {
            Interpret(@"var a = 1, b = true, c = false, d = 'd', e;")
                .Equal("a", 1)
                .Equal("b", true)
                .Equal("c", false)
                .Equal("d", "d")
                .Equal("e", null);
        }

        [Test]
        public void Return()
        {
            Interpret(@"var a;
                        var f = func() { if (true) return; a = 1;};
                        f();").Null("a");
        }

        [Test]
        public void Identifiers()
        {
            Interpret(@"var _a = 1;
                        var b$ = 2;
                        $_ = 3;")
                .Equal("_a", 1)
                .Equal("b$", 2)
                .Equal("$_", 3);

            Interpret(@"var a = {$: 1, $_: 2, _: 3, 4: 4};")
                .Equal("a.$", 1)
                .Equal("a.$_", 2)
                .Equal("a._", 3)
                .Equal("a.4", 4);
        }

        [Test]
        public void DeepCallExpression()
        {
            Interpret(
                @"func f(a) {
                    return func(a) {
                        return func(a) {
                            return a;
                        };
                    };
                }
                var a = f(1)(2)(3);")
            .Equal("a", 3);

            Interpret(
                @"f.o = {
                    f1: func(a) {
                        return {
                            f2: func(a) {
                                return {
                                    a: a
                                };
                            }
                        };
                    }
                  };
                  a = f.o.f1(1).f2(2).a;")
            .Equal("a", 2);

            Interpret(
                @"a = (func(a){ 
                          return func(a) {
                              return a;
                          }; 
                       })(1)(2);")
            .Equal("a", 2);

            Interpret(
                @"a = {o1: {o2: [{o3: func(a) {return a;}}]}}.o1.o2[0].o3(10);")
            .Equal("a", 10);

            Interpret(
                @"a = [1, 2, [3, [func(a) {return a;}]]] [2][1][0]({a: 100}).a;")
            .Equal("a", 100);
        }

        [Test]
        public void OptionalReturn()
        {
            Interpret(
                @"var a = (func(a){ a;})(100);")
            .Equal("a", 100);

            Interpret(
                @"a = (func(){ 1+2;})();")
            .Equal("a", 3);

            Interpret(
                @"a = (func(a){ if (a) 1; else 2;})(false);")
            .Equal("a", 2);

            Interpret(
                @"a = (func(a){ {a: a};})(7).a;")
            .Equal("a", 7);

            Interpret(
                @"a = (func(){ return 1; 2;})();")
            .Equal("a", 1);

            Interpret(
                @"a = (func(){ for (var i in [1, 2, 3]) i; })();")
            .Equal("a", 3);

            Interpret(
                @"a = (func(){ var i = 0; while (i++ < 10) i; })();")
            .Equal("a", 10);
        }

        [Test]
        public void OptionalParameter()
        {
            Interpret(
                @"var a = (func(p1, p2){ return p2; })(1);")
            .Null("a");
        }

        [Test]
        public void ArgumentExpansion()
        {
            Interpret(
                @"func f(p1, p2) {p1 + p2;};
                  var a = f(*[2, 3]);")
            .Equal("a", 5);

            Interpret(
                @"var o = {f: func(p1, p2) {p1 + p2;}};
                  var ar = [2, 3];
                  a = o.f(*ar);")
            .Equal("a", 5);
        }

        [Test]
        public void ThisAsFunction()
        {
            Interpret(
                @"Bike.Function.forward = func(*args){
                     this(*args);
                  };
                  var r = (func(a, b) {
                    a + b;
                  }).forward(1, 2);")
            .Equal("r", 3);
        }

        [Test]
        public void ArgumentCollapse()
        {
            Interpret(
                @"func f1(*p) {p;};
                  var a = f1();")
            .Null("a");

            Interpret(
                @"func f2(*p) {p[0];};
                  a = f2(1);")
            .Equal("a", 1);

            Interpret(
                @"func f3(*p) {p[0] + p[1] + p[2];};
                  a = f3(1, 2, 3);")
            .Equal("a", 6);
            
            Interpret(
                @"a = f3(*[1, 2, 3]);")
            .Equal("a", 6);
        }

        [Test]
        public void LoneExpressions()
        {
            Interpret(@"1;");
            Interpret(@"1.1;");
            Interpret(@"true;");
            Interpret(@"false;");
            Interpret(@"'str';");
            Interpret(@"null;");
            Interpret(@"[];");
            Interpret(@"[1, 2, [3, [5]]];");
            Interpret(@"[1, 2, [3, [5]]][2][1][0];");
            Interpret(@"{};");
            Interpret(@"{o1: {o2: {o3: 1}}};");
            Interpret(@"{o1: {o2: {o3: 1}}}.o1.o2.o3;");
        }

        [Test]
        public void DeepMemberExpression()
        {
            Interpret(@"var a = {o1: {o2: {o3: 1}}}.o1.o2.o3;")
                .Equal("a", 1);

            Interpret(@"a = [1, 2, [3, [5]]] [2][1][0];")
                .Equal("a", 5);

            Interpret(@"a = {o1: [{o2: [ {o3: [100]} ] }]}.o1[0].o2[0].o3[0] ;")
                .Equal("a", 100);
        }

        [Test]
        public void DeepAssign()
        {
            Interpret(
                @"var obj = {o1: [{o2: [ {o3: [100]} ] }]};
                  obj.o1[0] = 10;
                  var a = obj.o1[0];")
            .Equal("a", 10);

            Interpret(
                @"obj = {o1: [{o2: [ {o3: [100]} ] }]};
                  obj.o1[0].o2 = 100;
                  a = obj.o1[0].o2;")
            .Equal("a", 100);

            Interpret(
                @"obj = {o1: [{o2: [ {o3: [100]} ] }]};
                  obj.o1[0].o2[0] = 1000;
                  a = obj.o1[0].o2[0];")
            .Equal("a", 1000);

            Interpret(
                @"obj = {o1: [{o2: [ {o3: [100]} ] }]};
                  obj.o1[0].o2[0].o3[0] = 10000;
                  a = obj.o1[0].o2[0].o3[0];")
            .Equal("a", 10000);

            Interpret(
                @"var ctx = {value: 1};
                  obj = {o1: [{o2: func() {
                                            ctx;
                                       } 
                                  }
                                 ]
                            };
                  obj.o1[0].o2().value = 100;
                  a = ctx.value;")
            .Equal("a", 100);
        }

        [Test]
        public void Boolean()
        {
            Interpret(@"var a = true && false;").Equal("a", false);
            Interpret(@"a = true && true;").Equal("a", true);
            Interpret(@"a = false && false;").Equal("a", false);

            Interpret(@"a = true || false;").Equal("a", true);
            Interpret(@"a = true || true;").Equal("a", true);
            Interpret(@"a = false || false;").Equal("a", false);

            Interpret(@"a = null && null;").Null("a");
            Interpret(@"a = null || null;").Null("a");
            Interpret(@"a = null || 1;").Equal("a", 1);
            Interpret(@"a = 1 && 1;").Equal("a", 1);
            Interpret(@"a = null;
                        a = a || {a: 1};").Equal("a.a", 1);
            Interpret(@"a = null;
                        a ||= {a: 1};").Equal("a.a", 1);
            Interpret(@"a = null;
                        a &&= {a: 1};").Equal("a", null);
            Interpret(@"a = {};
                        a &&= {a: 1};").Equal("a.a", 1);
            Interpret(@"var called = false;
                        func f() {called = true}
                        a = true || f();").Equal("called", false);
            Interpret(@"called = false;
                        a = true;
                        a ||= f();").Equal("called", false);
            Interpret(@"called = false;
                        a = false && f1();").Equal("called", false);
            Interpret(@"called = false;
                        a = false;
                        a &&= f1();").Equal("called", false);
        }

        [Test]
        public void Unary()
        {
            Interpret(@"var a = 0; var b = a++;").Equal("a", 1).Equal("b", 0);
            Interpret(@"a = 0; b = ++a;").Equal("a", 1).Equal("b", 1);
            Interpret(@"a = 0; b = a--;").Equal("a", -1).Equal("b", 0);
            Interpret(@"a = 0; b = --a;").Equal("a", -1).Equal("b", -1);
            Interpret(@"a = true; b = !a;").Equal("a", true).Equal("b", false);
            Interpret(@"a = !!!false;").Equal("a", true);
            Interpret(@"a = !!!null;").Equal("a", true);
            Interpret(@"a = !!!1;").Equal("a", false);
            Interpret(@"a = +1 + -2;").Equal("a", -1);
            Interpret(@"a = -1 + +2;").Equal("a", 1);
        }

        [Test]
        public void Comparisons()
        {
            Interpret(@"var a = 1 == 1;").Equal("a", true);
            Interpret(@"a = 'a' == 'a';").Equal("a", true);
            Interpret(@"a = null == null;").Equal("a", true);
            Interpret(@"a = false == false;").Equal("a", true);
            Interpret(@"a = true == true;").Equal("a", true);

            Interpret(@"a = 1 != 2;").Equal("a", true);
            Interpret(@"a = 'a' != 'b';").Equal("a", true);
            Interpret(@"a = null != null;").Equal("a", false);
            Interpret(@"a = false != true;").Equal("a", true);
            Interpret(@"a = true != true;").Equal("a", false);

            Interpret(@"a = 1 < 2;").Equal("a", true);
            Interpret(@"a = 2 <= 2;").Equal("a", true);
            Interpret(@"a = 2 > 2;").Equal("a", false);
            Interpret(@"a = 2 >= 2;").Equal("a", true);

            InterpretFail("TypeError",
                    @"a = 1 > 'str';",
                    @"a = true <= false;",
                    @"a = true >= 2;",
                    @"a = 'a' < 'b';");
        }

        [Test]
        public void Assignments()
        {
            Interpret(@"var a = 1;
                        var b = a * 2;
                        var c = a + b;
                        var d = a, e = b, f = c;")
                .Equal("a", 1).Equal("b", 2).Equal("c", 3).Equal("d", 1).Equal("e", 2).Equal("f", 3);

            Interpret(@"var g = a = b = c = d = e = f = 4;")
                .Equal("g", 4).Equal("a", 4).Equal("b", 4).Equal("c", 4).Equal("d", 4).Equal("e", 4).Equal("f", 4);

            Interpret(@"a = 1; a += 1;").Equal("a", 2);
            Interpret(@"a = 1; a -= 1;").Equal("a", 0);
            Interpret(@"a = 1; a *= 2;").Equal("a", 2);
            Interpret(@"a = 1; a /= 2;").Equal("a", 0.5);
            Interpret(@"a = 1; a %= 2;").Equal("a", 1);

            Interpret(@"a = true; a &&= false;").Equal("a", false);
            Interpret(@"a = true; a ||= false;").Equal("a", true);
        }

        [Test]
        public void Additive()
        {
            Interpret(@"var a = 1 + 2 - 0.5;").Equal("a", 2.5);
            Interpret(@"a = 'str' + 2;").Equal("a", "str2");
            Interpret(@"a = 1 + 'str' + 2;").Equal("a", "1str2");
            Interpret(@"a = true + 'str';").Equal("a", "Truestr");
            InterpretFail("TypeError", 
                    @"a = true - false;",
                    @"a = true + false;",
                    @"a = 'a' - 'b';");
        }

        [Test]
        public void Multiplicative()
        {
            Interpret(@"var a = 1 + (4*5/2)%3;").Equal("a", 2);
            Interpret(@"a = ((2.5*10 - 1)/4) % 4;").Equal("a", 2);
            Interpret(@"a = ((4-1)/2).floor();").Equal("a", 1);
            InterpretFail("TypeError", 
                    @"a = '1' * 2;",
                    @"a = null / 2;",
                    @"a = 1 + 2 % null;",
                    @"a = true / false;",
                    @"a = true % 2;");
        }

        [Test]
        public void Tertiary()
        {
            Interpret(@"var a = true ? true : false;").Equal("a", true);
            Interpret(@"a = false ? true : false;").Equal("a", false);
            Interpret(@"a = false ? true : true ? true : false;").Equal("a", true);
            Interpret(@"var count = 10;
                        a = count == null ? 0 : count;").Equal("a", 10);
        }

        [Test]
        public void If()
        {
            Interpret(@"var a, b; if (true) a = true;")
                .Equal("a", true);

            Interpret(@"a = true;
                        b = false;
                        if (a) b = true;")
                .Equal("a", true).Equal("b", true);

            Interpret(@"a = true;
                        b = false;
                        if (a == false) b = true;")
                .Equal("a", true).Equal("b", false);

            Interpret(@"a = true;
                        if (a) b = true;
                        else   b = false;")
                .Equal("a", true).Equal("b", true);

            Interpret(@"a = true;
                        if (a == false) b = true;
                        else   b = false;")
                .Equal("a", true).Equal("b", false);

            Interpret(@"a = 3;
                        if (a == 1) b = 1;
                        else if (a == 2) b = 2;
                        else if (a == 3) { b = 3; }")
                .Equal("a", 3).Equal("b", 3);

            Interpret(@"var c = false; a = 1, b = 2;
                        if (a == 1) {
                            if (b == 1) {
                            } else if (b == 2) {
                                c = true;
                            }
                        };")
                .Equal("a", 1).Equal("b", 2).Equal("c", true);
           
        }

        [Test]
        public void Exec()
        {
            Interpret(@"var a = exec '1;';")
                .Equal("a", 1);

            Interpret(@"a = exec '(func(a) {a + a;})(2);';")
                .Equal("a", 4);

            Interpret(@"var code = 'a + a;';
                        a = 2;
                        var b = exec code;")
                .Equal("b", 4);

            Interpret(@"code = exec 'func f(a) {a + a;}';
                        a = code is Bike.Function;
                        b = code(2);")
                .Equal("a", true)
                .Equal("b", 4);

            Interpret(@"exec 'a = 12;';")
                .Equal("a", 12);

            Interpret(@"exec 'a = 12;';")
                .Equal("a", 12);

            Interpret(@"a = 1;
                        (func() {a = 2; b = exec 'a;';})();")
                .Equal("b", 2);
        }

        [Test]
        public void While()
        {
            Interpret(@"var a = 0, b, i, j, sum;
                        while (a < 10) a++;")
                .Equal("a", 10);

            Interpret(@"a = null;
                        while (a != null);
                        b = true;")
                .Equal("b", true);

            Interpret(@"a = 0, b = 10;
                        while (a < 10 && b > 0) { a++; b--; }")
                .Equal("a", 10).Equal("b", 0);

            Interpret(@"i = 0, j = 0, a = 0;
                        while (i < 10) {
                            j = 0;
                            while (j < 10) {   
                                a++;
                                j++;
                            } 
                            i++;
                        }").Equal("a", 100);

            Interpret(@"a = 0;
                        while (a < 10) { a++; if (a == 5) break; }")
                .Equal("a", 5);


            Interpret(@"a = 0;
                        while (++a < 10) ;")
                .Equal("a", 10);

            Interpret(@"a = 0;
                        while (a++ < 10) ;")
                .Equal("a", 11);

            Interpret(@"a = 0, sum = 0;
                        while (a < 10) { a++; if (a < 3) next; sum++; }")
                .Equal("a", 10)
                .Equal("sum", 8);
        }

        [Test]
        public void ForIn()
        {
            Interpret(@"var a = [1, 2, 3], sum = 0;
                        for (var i in a) sum += i;")
                .Equal("sum", 6);

            Interpret(@"a = [1, 2, 3], sum = 0;
                        for (var i in a) { if (i == 3) break; sum += i;}")
                .Equal("sum", 3);

            Interpret(@"a = [1, 2, 3], sum = 0;
                        for (var i in a) { if (i == 1) next; sum += i;}")
                .Equal("sum", 5);

            Interpret(@"var p = {age: 10, salary: 200};
                        sum = 0;
                        for (var x in p) if (p[x] is Bike.Number) sum += p[x];")
                .Equal("sum", 210);
        }

        [Test]
        public void Scopes()
        {
            Interpret(@"var a = false, r, sum; 
                        if (true) {a = true;}")
                .Equal("a", true);

            Interpret(@"a = true; 
                        if (a) {var a = false;}")
                .Equal("a", true);

            Interpret(@"a = [1, 2, 3], sum = 0;
                        for (var i in a) sum += i;")
                .NotExist("i")
                .Equal("sum", 6);

            Interpret(@"a = 0, r;
                        while (a < 10) { 
                            var i; 
                            if (!i) { i = ++a; r = i; }
                        }")
                .Equal("r", 10);

            Interpret(
                @"(func() {
                     $PATH = 'a';
                     (func(){$PATH = 'b';})();
                  })();")
                .Equal("$PATH", "b");
        }

        [Test]
        public void Switch()
        {
            Interpret(
                @"var a = 1; 
                  var branch;
                  switch(a) {
                    case 0: branch = 0; break;
                    case 1: branch = 1; break;
                    default: branch = 2; break;
                  }").Equal("branch", 1);

            Interpret(
                @"a = 1; 
                  branch;
                  switch(a) {
                    case 0: branch = 0; break;
                    case a: branch = 1; break;
                    default: branch = 2; break;
                  }").Equal("branch", 1);

            Interpret(
                @"a = 'c'; 
                  branch;
                  switch(a) {
                    case 'a': branch = 'a'; break;
                    case 'b': branch = 'b'; break;
                    default: branch = 'c'; break;
                  }").Equal("branch", "c");

            Interpret(
                @"a = 1;
                  var sum = 0; 
                  switch(a) {
                    case 0: sum += 1;
                    case 1: sum += 2;
                    default: sum += 3;
                  }").Equal("sum", 5);

            Interpret(
                @"func f(a) {
                      switch(a) {
                        case 1, 2: return a;
                        default: return 3;
                      }
                  }
                  a = f(1);
                  var b = f(2);
                ").Equal("a", 1).Equal("b", 2);

            InterpretFail("AlreadyDefinedError", 
                @"a = 1; 
                  switch(a) {
                    case 0: var b = 1;
                    case 1: var b = 2;
                    default: var b = 3;
                  }");
        }

        [Test]
        public void TryRescue()
        {
            Interpret(
                @"var r, f;
                  try {
                     throw 1;
                  } rescue {
                     r = 'error';
                  }").Equal("r", "error");

            Interpret(
                @"try {
                     throw 'error';
                  } rescue e {
                     r = e;
                  } finally {
                     f = 'finally';
                  }").Equal("r", "error").Equal("f", "finally");

            Interpret(
                @"try {
                     var a = 1/0;
                  } rescue e {
                     r = e;
                  }").NotNull("r");

            Interpret(
                @"var e; r = null;
                  while (true) {
                      try {
                         break;
                      } rescue {
                         r = 'rescue';
                      } finally {
                         f = 'finally';
                      }
                      e = 'end';
                  }").Equal("f", "finally").Equal("r", null).Equal("e", null);


            Interpret(
                @"var FileError = {msg: 'file error'};
                  try {
                     throw FileError.clone();
                  } rescue e {
                     if (e is FileError)
                        r = e.msg;
                  }").Equal("r", "file error");
        }
    }
}
