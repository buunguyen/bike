using System;

namespace Bike.Interpreter
{
    public class InterpreterException : Exception
    {
        public InterpreterException(string msg) : this(msg, null)
        { }

        public InterpreterException(string msg, Exception inner) : base(msg, inner)
        { }
    }
}
