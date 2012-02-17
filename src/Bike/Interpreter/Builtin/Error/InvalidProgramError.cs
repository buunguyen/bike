namespace Bike.Interpreter.Builtin
{
    public class InvalidProgramError : Error
    {
        public InvalidProgramError(string message)
            : base(message, ResolvePrototype("InvalidProgramError"))
        {
        }
    }
}
