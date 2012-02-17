using System.Reflection;

namespace Bike.Interpreter.Builtin
{
    using System;

    public class ClrError : Error
    {
        public ClrError(string msg)
            : base(msg, ResolvePrototype("ClrError"), null)
        {
        }

        public ClrError(Exception inner)
            : base(Unwrap(inner).Message, ResolvePrototype("ClrError"), Unwrap(inner))
        {
            Type = new BikeString(Unwrap(inner).GetType().FullName);
        }

        private static Exception Unwrap(Exception ex)
        {
            return ex is TargetInvocationException && ex.InnerException != null 
                ? ex.InnerException 
                : ex;
        }
    }
}
