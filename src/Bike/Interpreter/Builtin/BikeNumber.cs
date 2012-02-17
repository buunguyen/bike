namespace Bike.Interpreter.Builtin
{
    public class BikeNumber : BikeObject
    {
        public decimal Value { get { return (decimal)Members[InterpreterHelper.SpecialSuffix + "native"]; } }

        public BikeNumber(decimal value)
            : base(InterpretationContext.NumberBase)
        {
            Members[InterpreterHelper.SpecialSuffix + "native"] = value;
        }
    }
}
