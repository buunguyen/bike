//namespace Bike.Interpreter.Builtin
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Text;
//
//    // TODO: how to make bike code invoke error constructor?
//    public class Error : BikeObject
//    {
//        public Exception Cause
//        {
//            get { return (Exception)Members["cause"]; }
//            protected set { Members["cause"] = value; }
//        }
//
//        public BikeString Type
//        {
//            get { return (BikeString)Members["type"]; }
//            protected set { Members["type"] = value; }
//        }
//
//        public Error(string message) : this(message, ResolvePrototype("Error")) {}
//
//        protected Error(string message, BikeObject prototype) : this(message, prototype, null) {}
//
//        protected Error(string message, BikeObject prototype, Exception cause)
//            : base(prototype)
//        {
//            Members[InterpreterHelper.SpecialSuffix + "is_error"] = new BikeBoolean(true);
//            Members["message"] = new BikeString(message);
//            Members["cause"] = cause;
//            Members["stack_trace"] = BuildStackTrace();
//        }
//
//        protected static BikeObject ResolvePrototype(string objName)
//        {
//            var ns = (BikeObject) InterpretationContext.Instance.GlobalFrame.Resolve("Bike");
//            if (ns.Exist(objName))
//                return (BikeObject) ns.Resolve(objName);
//            throw new InterpreterException("Cannot load core library");
//        }
//
//        internal static BikeString BuildStackTrace()
//        {
//            var lastLocation = InterpretationContext.Instance.CurrentLocation;
//            var callStack = new List<ScopeFrame>();
//            var currentFrame = InterpretationContext.Instance.CurrentFrame;
//            while (currentFrame != null)
//            {
//                if (currentFrame.Func != null || currentFrame.IsGlobal)
//                    callStack.Add(currentFrame);
//                currentFrame = currentFrame.Caller;
//            }
//
//            var sb = new StringBuilder();
//            foreach (var frame in callStack)
//            {
//                var at = frame.IsGlobal
//                             ? "<Global>"
//                             : frame.Func.Description;
//                sb.AppendFormat("   at {0} in {1}:line {2}", at, lastLocation.FilePath, lastLocation.Line)
//                    .AppendLine();
//                lastLocation = frame.Location;
//            }
//            return new BikeString(sb.ToString());
//        }
//    }
//}
