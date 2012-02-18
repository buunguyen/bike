namespace Bike.Test
{
    using System;
    using System.Threading;
    using NUnit.Framework;

    [TestFixture]
    public class InteropTest : BaseInterpreterTest
    {

        [Test]
        public void Generics()
        {
            Interpret(@"load 'Bike.Test';
                        System.Collections.Generic.List<System.String>;
                        System.Collections.Generic.Dictionary<System.Collections.Generic.List<System.Int32>, System.Int32>;");

            Interpret(@"var isType = Test.Generic<System.Int32>.Value is Bike.Number;")
                .Equal("isType", true);

            Interpret(@"var val = Test.Generic<System.String>.StaticMethod<System.Int32>(1);")
                .Equal("val", 1);

            Interpret(@"val = Test.Generic<System.Int32>().InstanceMethod<System.String>('hello');")
                .Equal("val", "hello");

            Interpret(@"var type = System.Collections.Generic.List<System.String>;
                        var obj = System.Collections.Generic.List<System.String>();
                        isType = obj is type && obj is System.Collections.Generic.List<System.String>;")
                .Equal("isType", true);

            Interpret(@"obj = type();
                        isType = obj is System.Collections.Generic.List<System.String>;")
                .Equal("isType", true);

            Interpret(@"var arr = System.Collections.Generic.List<System.String>();
                        var a;
                        arr.Add('str');
                        try {
                            arr.Add(1);
                        } rescue {
                            a = true;
                        }
                        var b = arr is System.Collections.Generic.List<System.String>;")
                .Equal("a", true)
                .Equal("b", true);
            Interpret(@"arr = System.Collections.Generic.Dictionary<
                                        System.String, System.Int32>();
                        a = false;
                        arr.Add('str', 1);
                        try {
                            arr.Add(1, 1);
                        } rescue {
                            a = true;
                        };")
                .Equal("a", true);
            Interpret(@"arr = System.Collections.Generic.Dictionary
                                  <System.Collections.Generic.List<System.Int32>, 
                                   System.Int32>();
                        a = false;
                        arr.Add(System.Collections.Generic.List<System.Int32>(), 1);
                        try {
                            arr.Add(1, 1);
                        } rescue {
                            a = true;
                        };")
                .Equal("a", true);
        }

        [Test]
        public void TypeInstanceMembers()
        {
            Interpret(@"var baseType = System.Type;
                        var type = System.Threading.Thread;
                        var name = type.FullName;
                        var isClass = type.IsClass;
                        var ofBaseType = type is System.Type;")
                                  .Equal("name", "System.Threading.Thread")
                                  .Equal("isClass", true)
                                  .Equal("ofBaseType", true);
        }

        [Test]
        public void Enum()
        {
            Interpret(@"var t = System.Threading.Thread(func() {});
                        var priority = System.Threading.ThreadPriority.Lowest;
                        t.Priority = priority;
                        var threadPriority = t.Priority;")
                    .Equal("priority", ThreadPriority.Lowest)
                    .Equal("threadPriority", ThreadPriority.Lowest);
        }

        [Test]
        public void Static()
        {
            Interpret(@"var m = System.Math.Max(1, 2);").Equal("m", 2);

            Interpret(@"System.Console.WriteLine('Hello Bike');");

            Interpret(@"System.Console.WriteLine(null);");

            Interpret(@"var out = System.Console.Out;").NotNull("out");

            Interpret(@"var math = System.Math;").NotNull("math");

            Interpret(@"m = math.Min(1, 2);").Equal("m", 1);

            Interpret(@"var a = {}; 
                        a.m = System.Math; 
                        m = a.m.Min(1, 2);").Equal("m", 1);

            Interpret(@"a.m = System.Math; 
                        m = a.m.Min(1, 2);").Equal("m", 1);
        }

        [Test]
        public void Is()
        {
            Interpret(@"load 'Bike.Test';
                        var ip = Test.Person('bike', 0, 'kiki')
                                 is Test.Person;")
                .Equal("ip", true);
          
            Interpret(@"var r;
                        var type = System.Exception;
                        try {
	                        throw System.Exception();
                        } rescue e {
							println('*******');
							println(e);
                            println(e.cause);
                            if (e.cause is type) r = true;
                            if (e.cause is System.Exception) r &&= true;
                        }").Equal("r", true);

            Interpret(@"try {
	                        throw System.ApplicationException();
                        } rescue e {
	                        r = e is Bike.Number;
                        }").Equal("r", false);
        }

        [Test]
        public void Load()
        {
            Interpret(@"load 'System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a';");
        }

        [Test]
        public void Namespace()
        {
            Interpret(@"var d = System.Int32.Parse('1');
                        System.Console.WriteLine(d);
                        System.Console.Out.WriteLine(d);")
                .Equal("d", 1);
        }

        [Test]
        public void Indexer()
        {
            Interpret(@"load 'Bike.Test';
                        var person = Test.Person('bike', 0, 'kiki');
                        var a = person['bike', 0];
                        person['bike', 0] = false;
                        var b = person.Name;")
                .Equal("a", true)
                .Equal("b", "Indexer");
        }

        [Test]
        public void Primitives()
        {
            Interpret(@"var d = System.Int32.Parse('1');")
                .Equal("d", 1);

            Interpret(@"var min = System.Int32.MinValue;")
                    .Equal("min", Int32.MinValue);
        }

        [Test]
        public void TypeInterop()
        {
            Interpret(
                @"load 'Bike.Test';
                  var person = Test.Person('bike', 0, 'kiki');
                  var a = person.TakeNumber(1);")
                .Equal("a", 1);

            Interpret(
                @"a = person.TakeNumber(2.0);")
                .Equal("a", 1);

            InterpretFail("ClrError", @"person.TakeNumber(3.3);");
        }

        [Test]
        public void Delegates()
        {
            Interpret(
                @"load 'Bike.Test';
                  var person = Test.Person();
                  var a = person.action(2);
                  var del = person.action;
                  var b = del(3);
                  var c = Test.Person().action(4);
                  var obj = {del: del};
                  var d = obj.del(5);")
                .Equal("a", 4)
                .Equal("b", 9)
                .Equal("c", 16)
                .Equal("d", 25);
        }

        [Test]
        public void InstanceEventHandling()
        {
            Interpret(
                @"load 'Bike.Test';
                  var person = Test.Person();
                  person.action = func(arg) {arg * arg;};
                  var a = person.action(2);")
                .Equal("a", 4);

            Interpret(
                @"person = Test.Person();
                  person.AssignDelegate(func(arg) {arg * arg;});
                  a = person.action(2);")
                .Equal("a", 4);

            Interpret(
                @"person = Test.Person('bike', 11, 'kiki');
                  var sum = 0;
                  person.action += func(arg) {sum += this.Age;};
                  person.action += func(arg) {sum += (arg * arg);};
                  a = person.action(2);")
                .Equal("a", 15)
                .Equal("sum", 15);

            Interpret(
                @"person = Test.Person();
                  sum = 0;
                  person.AddDelegate(func(arg) {sum += arg;});
                  person.AddDelegate(func(arg) {sum += (arg * arg);});
                  a = person.action(2);")
                .Equal("a", 6)
                .Equal("sum", 6);

            Interpret(
                @"person = Test.Person('bike', 11, 'kiki');
                  sum = 0;
                  person.eventAction += func(arg) {sum += this.Age;};
                  person.eventAction += func(arg) {sum += (arg * arg);};
                  a = person.RunEvent(2);")
                .Equal("a", 15)
                .Equal("sum", 15);

            Interpret(
                @"person = Test.Person();
                  sum = 0;
                  person.AddEvent(func(arg) {sum += arg;});
                  person.AddEvent(func(arg) {sum += (arg * arg);});
                  a = person.RunEvent(2);")
                .Equal("a", 6)
                .Equal("sum", 6);
            
            Interpret(
                @"person = Test.Person('bike', 11, 'kiki');
                  person.action = func() { this.Age; };
                  a = person.action(0);")
                .Equal("a", 11);
        }

        [Test]
        public void StaticEventHandling()
        {
            Interpret(
                @"load 'Bike.Test';
                  Test.Person.staticAction = func(arg) {arg * arg;};
                  var a = Test.Person.staticAction(2);")
                .Equal("a", 4);

            Interpret(
                @"Test.Person.StaticAssignDelegate(func(arg) {arg * arg;});
                  a = Test.Person.staticAction(2);")
                .Equal("a", 4);

            Interpret(
                @"var sum = 0;
                  Test.Person.staticAction += func(arg) {sum += arg;};
                  Test.Person.staticAction += func(arg) {sum += (arg * arg);};
                  a = Test.Person.staticAction(2);")
                .Equal("a", 6)
                .Equal("sum", 6);

            Interpret(
                @"sum = 0;
                  Test.Person.StaticAddDelegate(func(arg) {sum += arg;});
                  Test.Person.StaticAddDelegate(func(arg) {sum += (arg * arg);});
                  a = Test.Person.staticAction(2);")
                .Equal("a", 12) // 12 because the previous handlers are still intact
                .Equal("sum", 12);

            Interpret(
                @"sum = 0;
                  Test.Person.staticEventAction += func(arg) {sum += arg;};
                  Test.Person.staticEventAction += func(arg) {sum += (arg * arg);};
                  a = Test.Person.StaticRunEvent(2);")
                .Equal("a", 6)
                .Equal("sum", 6);

            Interpret(
                @"sum = 0;
                  Test.Person.StaticAddEvent(func(arg) {sum += arg;});
                  Test.Person.StaticAddEvent(func(arg) {sum += (arg * arg);});
                  a = Test.Person.StaticRunEvent(2);")
                .Equal("a", 12)
                .Equal("sum", 12);
        }

        [Test]
        public void ArrayMarshall()
        {
            Interpret(@"var arg = Test.Person('bike', 0, 'kiki')
                                  .Bounce([1, 2, [3]]);
                        var a = arg is Bike.Array;
                        var b = arg[0] is Bike.Number;
                        var c = arg[2] is Bike.Array;
                        var d = arg[2][0] is Bike.Number;")
                .Equal("a", true)
                .Equal("b", true)
                .Equal("c", true)
                .Equal("d", true);

            Interpret(@"a = Test.Person('bike', 0, 'kiki')
                                  .TakeArray([1, 2, 3]);")
                .Equal("a", true);

            Interpret(@"a = Test.Person('bike', 0, 'kiki')
                                  .TakeArray([Test.Person('bike', 0, 'kiki')]);")
                .Equal("a", true);

            Interpret(@"a = Test.Person('bike', 0, 'kiki')
                                  .TakeArray(['a']);")
                .Equal("a", true);
        }

        [Test]
        public void Instance()
        {
            Interpret(@"load 'Bike.Test';
                        var person = Test.Person('bike', 0, 'kiki');
                        person.Name = 'John';
                        var name = person.Name;
                        person.Age = 10;
                        var age = person.Age;
                        var dogName = person.Pet.Name;")
                .Equal("name", "John")
                .Equal("age", 10)
                .Equal("dogName", "kiki");

            Interpret(@"func f() {
                            return person;
                        };
                        f().Pet.Name = 'lulu';
                        dogName = person.Pet.Name;")
                .Equal("dogName", "lulu");

            Interpret(@"f().Pet.Name = 'lulu-kiki';
                        dogName = f().Pet.Name;")
                .Equal("dogName", "lulu-kiki");

            Interpret(@"var arr = [{m: f}];
                        arr[0].m().Pet.Name = 'll';
                        dogName = arr[0].m().Pet.Name;")
                .Equal("dogName", "ll");

            Interpret(@"System.Console.Out.WriteLine('Hello Bike!');");

            Interpret(@"var out = System.Console.Out;
                        out.WriteLine('Hello Bike, again!');");
            Interpret(
                  @"var list = System.Collections.ArrayList();
                    for (var i in [1, 2, 3, 4]) 
                        list.Add('list item ' + i);
                    list.RemoveAt(1);
                    var i = 0;
                    while (i < list.Count)
                        System.Console.WriteLine(list[i++]);
                        
                    for (var i in [1, 2, 3]) 
                        list[i-1] = list[i-1] + ' (updated)';
                        
                    for (var obj in list)
                        System.Console.WriteLine(obj);
                    ");
        }
    }
}
