namespace Bike.Interpreter.Builtin
{
    public class AlreadyDefinedError : Error
    {
        public AlreadyDefinedError(string varName)
//            : base(string.Format("'{0}' is already defined", varName),
                   ResolvePrototype("AlreadyDefinedError"))
        {
            Members["var_name"] = new BikeString(varName);
        }
    }
}
