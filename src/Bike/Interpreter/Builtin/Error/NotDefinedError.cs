namespace Bike.Interpreter.Builtin
{
    public class NotDefinedError : Error
    {
        public NotDefinedError(string varName)
            : base(string.Format("'{0}' does not exist", varName),
                   ResolvePrototype("NotDefinedError"))
        {
            Members["var_name"] = new BikeString(varName);
        }
    }
}
