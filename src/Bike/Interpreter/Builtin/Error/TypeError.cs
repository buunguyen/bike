namespace Bike.Interpreter.Builtin
{
    public class TypeError : Error
    {
        public TypeError(string message)
            : base(message, ResolvePrototype("TypeError"))
        {
        }
    }
}
