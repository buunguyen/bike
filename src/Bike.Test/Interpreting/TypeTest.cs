namespace Bike.Test
{
    using NUnit.Framework;

    [TestFixture]
    public class TypeTest : BaseInterpreterTest
    {
        [Test]
        public void Objects()
        {
            Interpret(@"var p = {name: 'John', age: 10};")
                .NotNull("p")
                .Equal("p.name", "John")
                .Equal("p.age", 10);

            Interpret(@"var name = p.name;").Equal("name", "John");
            Interpret(@"var age = p.age;").Equal("age", 10);

            Interpret(@"p.name = 'Jane'; p.age = 15;")
                .Equal("p.name", "Jane").Equal("p.age", 15);
        }

        [Test]
        public void ObjectsAsHashes()
        {
            Interpret(
                    @"var map = {key1: 'value1', '2 key': 'value2', 'key3': 'value3'};
                      var v1 = map['key1'];
                      var v2 = map['2 key'];
                      var v3 = map.key3;")
                .Equal("v1", "value1")
                .Equal("v2", "value2")
                .Equal("v3", "value3");
        }

        [Test]
        public void ObjectMembers()
        {
            Interpret(
                    @"var obj = {a: 1, b: 2};
                      var c1 = obj.members().size();
                      var own1 = obj.has_member('a');
                      var own2 = obj.has_member('c');")
                .Equal("c1", 3)
                .Equal("own1", true)
                .Equal("own2", false);
        }

        [Test]
        public void NestedObjects()
        {
            Interpret(
                @"var n = 10, phone = {mobile: 903, home: 836};
                                var p = {
                                    name: 'John', 
                                    age: n,
                                    car: {
                                        brand: 'Toyota', 
                                        price: 69.5,
                                        distributor: {name: 'D1', phone: phone}
                                    }
                                };");
            Interpret(@"var brand = p.car.brand, 
                            price = p.car.price, 
                            dname = p.car.distributor.name,
                            dmobile = p.car.distributor.phone.mobile,
                            dhome = p.car.distributor.phone.home
                            ;")
                .Equal("brand", "Toyota")
                .Equal("price", 69.5)
                .Equal("dname", "D1")
                .Equal("dmobile", 903)
                .Equal("dhome", 836);

            Interpret(@"brand = p.car.brand = 'BMW';
                        price = p.car.price = 70;
                        dname = p.car.distributor.name = 'D2';
                        dmobile = p.car.distributor.phone.mobile = 908;
                        dhome = p.car.distributor.phone.home = 878;")
                .Equal("p.car.brand", "BMW")
                .Equal("price", 70)
                .Equal("p.car.distributor.name", "D2")
                .Equal("dmobile", 908)
                .Equal("p.car.distributor.phone.home", 878);
        }

        [Test]
        public void Equal()
        {
            Interpret(
                @"var e1 = {} == {};
                  var obj1 = {};
                  var obj2 = obj1; 
                  var e2 = obj1 == obj2;")
                .Equal("e1", false)
                .Equal("e2", true);
        }

        [Test]
        public void HashCode()
        {
            Interpret(
                @"var h1 = {}.hash_code();
                  var obj = {};
                  var h2 = obj.hash_code();
                  var h3 = obj.hash_code();
                  var e1 = h1 == h2;
                  var e2 = h2 == h3;")
                .Equal("e1", false)
                .Equal("e2", true);
        }

        [Test]
        public void Inheritance()
        {
            Interpret(@"var Person = {name: 'Person', age: 0};
                        var john = Person.clone();
                        var ns = {obj: Bike.Object};
                        var jip = john is Person;
                        var jio = john is ns.obj;")
                .NotNull("Person.==")
                .NotNull("Person.hash_code")
                .NotNull("Person.to_string")
                .Equal("john.name", "Person")
                .Equal("john.age", 0)
                .Equal("jip", true)
                .Equal("jio", true);

            Interpret(@"var jane = john.clone();
                        john.name = 'John';
                        john.age = 10;")
                .Equal("john.name", "John")
                .Equal("john.age", 10)
                .Equal("jane.name", "John")
                .Equal("jane.age", 10)
                .Equal("Person.name", "Person")
                .Equal("Person.age", 0);
        }

        [Test]
        public void ThisMess()
        {
            Interpret(@"var Person = {
                            name: 'Person', 
                            age: 0,
                            get_name: func() {
                                this.name;
                            }
                        };
                        var Employee = Person.clone();   
                        Employee.name = 'Employee';
                        Employee.to_string = func() {
                            this.get_name() + ' ' + this.age;
                        };
                        var john = Employee.clone();
                        john.name = 'John';
                        john.age = 11;
                        var str = john.to_string();")
                .Equal("str", "John 11");
        }

        [Test]
        public void ObjectOverride()
        {
            Interpret(@"Bike.Object.to_string = func() {this.name;};
                        Bike.Object.get_string = func() {'str';};
                        var blank = {};
                        var gs = blank.get_string();
                        blank.name = 'John';
                        var ts = blank.to_string();")
                .Equal("gs", "str")
                .Equal("ts", "John");
        }

        [Test]
        public void ConstructorIdiom()
        {
            Interpret(@"var Person = {
                            init: func(name, age) {
                                this.name = name;
                                this.age = age;
                                return this;
                            }
                        };
                        var john = Person.clone().init('John', 10);
                        var jane = Person.clone();
                        jane.init('Jane', 11);")
                .Equal("john.name", "John")
                .Equal("john.age", 10)
                .Equal("jane.name", "Jane")
                .Equal("jane.age", 11);
        }

        [Test]
        public void DirectPrototype()
        {
            Interpret(@"var ns = {};
                        ns.Person = {
                            init: func(name, age) {
                                this.name = name;
                                this.age = age;
                            }
                        };
                        var john = {};
                        john.prototype = ns.Person.clone(); # 1 level of indirection
                        john.init('John', 10);

                        var anotherJohn = {};
                        anotherJohn.prototype = john;")
                .Equal("john.name", "John")
                .Equal("john.age", 10)
                .Equal("anotherJohn.name", "John")
                .Equal("anotherJohn.age", 10);
        }

        [Test]
        public void Super()
        {
            Interpret(
                @"var parent = {name: 'parent'};
                  var child = parent.clone();
                  child.name = 'child';
                  var parent_name = child.prototype.name;
                  var child_name = child.name;")
                .Equal("parent_name", "parent")
                .Equal("child_name", "child");

            Interpret(
                @"parent = {id: '1', name: func() {'parent';}};
                  child = parent.clone();
                  child.id = 2;
                  child.name = func() {
                       'child ' + this.super('name');
                  };
                  var a = parent.name();")
                .Equal("a", "parent");

            Interpret("var b = child.name();")
                .Equal("b", "child parent");
        }

        [Test]
        public void Arrays()
        {
            Interpret(@"var a = [0, 1, 2, 3, 4];
                        var at1 = a[0];
                        var size = a.size();
                        var atn = a[size - 1];")
                .Equal("at1", 0)
                .Equal("size", 5)
                .Equal("atn", 4);

            Interpret(@"a = [0, [1]];
                        a[0] = 2;
                        a[1][0] = 2;")
                .Equal("a.0", 2)
                .Equal("a.1.0", 2);

            Interpret(
                @"a = [0, 1, 2, 3, [4, 5, [6]]];
                        at1 = a[4][0];")
                .Equal("at1", 4);

            Interpret(
                @"func f(array) {return array;};
                  var arr = f([0, 1, 2]);")
                .NotNull("arr");

            Interpret(
                @"var num = 1;
                  a = [num->11];
                  var isArray = a is Bike.Array;
                  size = a.size();")
                .Equal("isArray", true)
                .Equal("size", 10)
                .Equal("a.0", 1)
                .Equal("a.9", 10);
        }

        [Test]
        public void StringMethods()
        {
            Interpret(@"var se1 = 'John   '.trim_end();
                        var se2 = '  b John   aa '.trim_end(' ', 'a').trim_start(' ', 'b');
                        var s = 'John '.trim(' ');
                        var l = s.length();
                        var f = s.char(0);
                        var sub = s.sub(1, 2);
                        var low = s.lower();
                        var up = s.upper();
                        var i = s.index('hn');
                        var c = s.contains('oh');
                        var r = 'This is {0}!'.with('it');")
                .Equal("se1", "John")
                .Equal("se2", "John")
                .Equal("s", "John")
                .Equal("l", 4)
                .Equal("f", "J")
                .Equal("sub", "oh")
                .Equal("low", "john")
                .Equal("up", "JOHN")
                .Equal("i", 2)
                .Equal("c", true)
                .Equal("r", "This is it!");
          
            Interpret(
                @"s.firstCaseDown = func() {
                     return s.char(0).lower() + s.sub(1, s.length() - 1);
                  };
                  var uc = s.firstCaseDown();")
                .Equal("uc", "john");

            Interpret(@"var b1 = 'abc' == 'abc';")
                .Equal("b1", true);

            Interpret(@"var ts = 'abc'.to_string();")
                .Equal("ts", "abc");

        }

        [Test]
        public void NumberMethods()
        {
            Interpret(@"var val = '10.5'.to_number() == 10.5;")
                .Equal("val", true);

            Interpret(@"var sum = 0;
                        4.times(func(no) {sum += no;});")
                .Equal("sum", 6);

            Interpret(@"sum = 0;
                        1.upto(4, func(no) {sum += no;});")
                .Equal("sum", 6);

            Interpret(@"sum = 0;
                        4.downto(1, func(no) {sum += no;});")
                .Equal("sum", 9);
            
            Interpret(@"var i = 4.5;
                        var c = i.ceil();
                        var f = i.floor();")
                .Equal("c", 5)
                .Equal("f", 4);

            Interpret(@"var even = 4.even();
                        var odd = 5.odd();")
                .Equal("even", true)
                .Equal("odd", true);

            Interpret(@"var round = 4.123.round(2);")
                .Equal("round", 4.12);
        }

        [Test]
        public void AngryEqual()
        {
            Interpret(@"var b = false == false;").Equal("b", true);
            Interpret(@"b = false == true;").Equal("b", false);
            Interpret(@"b = 10 == 10;").Equal("b", true);
            Interpret(@"b = 10 == 11;").Equal("b", false);
            Interpret(@"b = 'ab' == 'ab';").Equal("b", true);
            Interpret(@"b = 'ab' == 'ba';").Equal("b", false);
        }

        [Test]
        public void ArrayMethods()
        {
            Interpret(@"var a = [];
                        var i = 0;
                        while (i < 10) a[i++] = i;
                        var size = a.size();
                        var last = a[size - 1];")
                    .Equal("size", 10)
                    .Equal("last", 9);

            Interpret(@"a = [];
                        a[3] = 1;
                        var first = a[0];
                        a[0] = 0;
                        var second = a[0];")
                .Null("first")
                .Equal("second", 0);

            Interpret(@"a = [1, 2, 3, 4];
                        var sum = 0;
                        a.each(func(ele) {sum += ele;});")
                .Equal("sum", 10);

            Interpret(@"a = [1, 2, 3, 4];
                        var f = a.filter(func(ele) {ele % 2 == 0;});
                        var sf = f.size();")
                .Equal("sf", 2);

            Interpret(@"a = [1, 2, 3, 4];
                        var has = a.has(3) || [1, 2, 3, 4].has(4);")
                .Equal("has", true);

            Interpret(@"a = [];
                        a.add(1);
                        var e = a[0] + 1;")
                .Equal("e", 2);
        }

        [Test]
        public void ArraysAndObjects()
        {
            Interpret(@"var p = {
                            phones: [{type: 'mobile', num: 903}, {type: 'work', num: 836}]
                        };
                        var a = [p, p];
                        var type1 = a[0].phones[0].type;
                        var num1 = a[0].phones[0].num;
                        var type2 = a[1].phones[1].type;
                        var num2 = a[1].phones[1].num;")
                .Equal("type1", "mobile")
                .Equal("num1", 903)
                .Equal("type2", "work")
                .Equal("num2", 836);

            Interpret(@"a[0].phones[0].type = 'work';
                        type1 = a[0].phones[0].type;
                        num1 = a[0].phones[0].num = 836;
                        type2 = a[1].phones[1].type = 'mobile';
                        num2 = a[1].phones[1].num = 903;")
                .Equal("type1", "work")
                .Equal("num1", 836)
                .Equal("type2", "mobile")
                .Equal("num2", 903);
        }

        [Test]
        public void Funcs()
        {
            Interpret(
                @"func add(a, b) {
                     return a + b;
                  }
                  var a = add(1, 2);
                  var b = add(a, 3);")
                  .Equal("a", 3)
                  .Equal("b", 6);

            Interpret(
                @"var ma = add, mb = add; 
                  var fe = ma == mb;")
                  .Equal("fe", true);

            Interpret(
                @"func fib(n) {
                     return n <= 1 ? n : (fib(n-1) + fib(n-2));
                  }
                  a = fib(10);")
                .Equal("a", 55);

            Interpret(
                @"a = (func(n) {
                     return n * n;
                  })(10);")
                .Equal("a", 100);

            Interpret(
                @"a = (func square(n) {
                     return n * n;
                  })(10);
                  b = square(5);")
                .Equal("a", 100)
                .Equal("b", 25);

            Interpret(
                @"a = 1;
                  var f = (func(b, c) {
                     var d = 4;
                     return func() {
                        var e = 5; 
                        return a + b + c + d + e;};
                  })(2, 3);");

            Interpret(@"var sum = f();")
                .Equal("sum", 15);
            Interpret(
                @"a = (func square2(n) {
                     return n * n;
                  })(10);
                  b = square2(5);")
                .Equal("a", 100)
                .Equal("b", 25);

            Interpret(
                @"func square3(n) {
                     return n * n;
                  }
                  func invoke(f, n) {
                     return f(n);
                  }
                  a = invoke(square3, 5);")
                .Equal("a", 25);
        }

        [Test]
        public void FuncsOnObjects()
        {
            Interpret(
                @"var p = (func() {
                     var age = 10;
                     return {
                        getAge: func() {return age;}
                     };
                  })();
                  var age = p.getAge();
                  var age2 = p['getAge']();")
                .Equal("age", 10)
                .Equal("age2", 10);

            Interpret(
                @"p = {
                     age: 10,
                     getAge: func() {return this.age;}
                  };
                  age = p.getAge();")
                .Equal("age", 10);

            Interpret(
                @"p.setAge = func(age) {this.age = age;};
                  p.setAge(1);
                  age = p.getAge();")
                .Equal("age", 1);

            Interpret(
                @"p.alias = func() {return this.getAge();};
                  age = p.alias();")
                .Equal("age", 1);

            Interpret(@"var p2 = {age: 5, getAge: p.getAge};
                        age = p2.getAge();").Equal("age", 5);
        }
    }
}
