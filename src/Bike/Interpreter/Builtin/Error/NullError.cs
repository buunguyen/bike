namespace Bike.Interpreter.Builtin
{
    public class NullError : Error
    {
        public NullError(string varName)
            : base(string.Format("{0} is null", varName),
                   ResolvePrototype("NullError"))
        {
            Members["var_name"] = new BikeString(varName);
        }
    }
}
