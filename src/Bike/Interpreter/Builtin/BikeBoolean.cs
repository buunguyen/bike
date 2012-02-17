namespace Bike.Interpreter.Builtin
{
    public class BikeBoolean : BikeObject
    {
        public bool Value
        {
            get
            {
                return (bool)Members[InterpreterHelper.SpecialSuffix + "native"];
            }
        }

        public BikeBoolean(bool value) : base(InterpretationContext.BooleanBase)
        {
            Members[InterpreterHelper.SpecialSuffix + "native"] = value;
        }
    }
}
