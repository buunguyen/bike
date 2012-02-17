namespace Bike.Interpreter.Builtin
{
    public class BikeString : BikeObject
    {
        public string Value { get { return (string)Members[InterpreterHelper.SpecialSuffix + "native"]; } }

        public BikeString(string value)
            : base(InterpretationContext.StringBase)
        {
            Members[InterpreterHelper.SpecialSuffix + "native"] = value;
        }
    }
}
