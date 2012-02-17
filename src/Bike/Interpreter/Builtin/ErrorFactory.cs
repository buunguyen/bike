namespace Bike.Interpreter.Builtin
{
    using System;
    using System.Collections.Generic;
    using System.Text;
	using System.Reflection;
	
    internal static class ErrorFactory
    {
        public static BikeObject CreateAlreadyDefinedError(string varName)
		{
			return CreateErrorFromType("AlreadyDefinedError", new BikeString(varName));
		}
		
        public static BikeObject CreateClrError(string message)
		{
			return CreateErrorFromType("ClrError", new BikeString(message), null);
		}
		
        public static BikeObject CreateClrError(Exception cause)
		{
            cause = cause is TargetInvocationException && cause.InnerException != null 
                ? cause.InnerException 
                : cause;
			return CreateErrorFromType("ClrError", null, cause);
		}
		
        public static BikeObject CreateError( string message )
		{
			return CreateErrorFromType("Error", new BikeString(message));
		}
		
        public static BikeObject CreateInvalidProgramError(string message)
		{
			return CreateErrorFromType("InvalidProgramError", new BikeString(message));
		}
		
        public static BikeObject CreateLoadError(string path)
		{
			return CreateErrorFromType("LoadError", new BikeString(path));
		}
		
        public static BikeObject CreateNotDefinedError(string varName)
        {
			return CreateErrorFromType("NotDefinedError", new BikeString(varName));
		}
		
        public static BikeObject CreateNullError(string varName)
		{
			return CreateErrorFromType("NullError", new BikeString(varName));
		}
		
        public static BikeObject CreateTypeError(string message)
		{
			return CreateErrorFromType("TypeError", new BikeString(message));
		}
		
		private static BikeObject CreateErrorFromType(string errorType, params object[] args)
		{
			var stackTrace = BuildStackTrace();
			var prototype = ResolvePrototype(errorType);
			var ctor = (BikeFunction)prototype.Resolve("create");
			var error = (BikeObject)InterpretationContext.Instance.Interpreter.CallBikeFunction(ctor, prototype, args);
			error.Assign("stack_trace", stackTrace);
			return error;
		}
		
		public static bool IsClrError(BikeObject bo)
		{
			return IsErrorOfType(bo, "ClrError");
		}
		
		public static bool IsNotDefinedError(BikeObject bo)
		{
			return IsErrorOfType(bo, "NotDefinedError");
		}
		
		public static bool IsAlreadyDefinedError(BikeObject bo)
		{
			return IsErrorOfType(bo, "AlreadyDefinedError");
		}
		
		public static bool IsTypeError(BikeObject bo)
		{
			return IsErrorOfType(bo, "TypeError");
		}
		
		public static bool IsErrorOfType(BikeObject bo, string errorType)
		{
            if (bo == null)
                return false;
		    var prototype = ResolvePrototype(errorType);
		    return prototype.IsPrototypeOf(bo).Value;
		}

        private static BikeObject ResolvePrototype(string objName)
        {
            var ns = (BikeObject) InterpretationContext.Instance.GlobalFrame.Resolve("Bike");
            if (ns.Exist(objName))
                return (BikeObject) ns.Resolve(objName);
            throw new InterpreterException("Cannot load core library");
        }
		
        internal static BikeString BuildStackTrace()
        {
            var lastLocation = InterpretationContext.Instance.CurrentLocation;
            var callStack = new List<ScopeFrame>();
            var currentFrame = InterpretationContext.Instance.CurrentFrame;
            while (currentFrame != null)
            {
                if (currentFrame.Func != null || currentFrame.IsGlobal)
                    callStack.Add(currentFrame);
                currentFrame = currentFrame.Caller;
            }

            var sb = new StringBuilder();
            foreach (var frame in callStack)
            {
                var at = frame.IsGlobal
                             ? "<Global>"
                             : frame.Func.Description;
                sb.AppendFormat("   at {0} in {1}:line {2}", at, lastLocation.FilePath, lastLocation.Line)
                    .AppendLine();
                lastLocation = frame.Location;
            }
            return new BikeString(sb.ToString());
        }
    }
}
