namespace Test
{
    public delegate int Op(int i);
    public class Person
    {
        public class Dog
        {
            public Dog(string dogType)
            {
                Name = dogType;
            }

            public string Name { get; set; }
        }

        public string Name { get; set; }
        public int Age { get; set; }
        public Dog Pet { get; set; }

        public Op action;
        public static Op staticAction;
        public event Op eventAction;
        public static event Op staticEventAction;

        public void AssignDelegate(Op op)
        {
            action = op;
        }

        public void AddDelegate(Op op)
        {
            action += op;
        }

        public void AddEvent(Op op)
        {
            eventAction += op;
        }

        public static void StaticAssignDelegate(Op op)
        {
            staticAction = op;
        }

        public static void StaticAddDelegate(Op op)
        {
            staticAction += op;
        }

        public static void StaticAddEvent(Op op)
        {
            staticEventAction += op;
        }

        public object RunEvent(int arg)
        {
            return eventAction(arg);
        }

        public static object StaticRunEvent(int arg)
        {
            return staticEventAction(arg);
        }

        public Person() : this (string.Empty, 10, string.Empty)
        {    
        }

        public Person(string name, int age, string dogName)
        {
            Name = name;
            Age = age;
            Pet = new Dog(dogName);
            action += (val) => val * val;
        }

        public object[] Bounce(object[] args)
        {
            return args;
        }

        public bool TakeArray(int[] args)
        {
            return true;
        }

        public bool TakeArray(char[] args)
        {
            return true;
        }

        public bool TakeArray(Person[] args)
        {
            return true;
        }

        public int TakeNumber(int i)
        {
            return 1;
        }

        public decimal TakeDecimal(decimal  d)
        {
            return d;
        }

        public bool this[string name, int age]
        {
            get { return Name == name && Age == age; }   
            set { Name = "Indexer";}
        }

        public override string ToString()
        {
            return Name + ": " + Age;
        }
    }
}
