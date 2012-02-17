using System;
using System.Threading;
using Bike.Interpreter.Builtin;
using Fasterflect;
using Fasterflect.Probing;

namespace Bike.Interpreter
{
    public partial class Interpreter
    {
        public void AddHandler(object target, string name, BikeFunction bikeFunc, Interpreter interpreter, bool isStatic)
        {
            var invoker = new BikeCallback(target, null, bikeFunc);
            var delegateType = isStatic
                ? ((Type)target).AddHandler(name, invoker.Callback)
                : target.AddHandler(name, invoker.Callback);
            invoker.ReturnType = delegateType.GetMethod("Invoke").ReturnType;
        }

        public class BikeCallback
        {
            private static readonly Cache<Thread, InterpretationContext> ContextLocal =
                new Cache<Thread, InterpretationContext>();

            private readonly Thread callingThread;
            public Type ReturnType;
            public readonly object Target;
            public readonly BikeFunction Function;

            public BikeCallback(object target, Type returnType, BikeFunction function)
            {
                callingThread = Thread.CurrentThread;
                ContextLocal[callingThread] = InterpretationContext.Instance;
                Target = target;
                ReturnType = returnType;
                Function = function;
            }

            public object Callback(object[] args)
            {
                bool newThread = false;
                if (callingThread != Thread.CurrentThread)
                {
                    newThread = true;
                    var callingInstance = ContextLocal[callingThread];
                    new InterpretationContext(callingInstance);
                    ContextLocal.ClearCollected();
                }

                Func<object> func = () =>
                {
                    var interpreter = InterpretationContext.Instance.Interpreter;
                    var result = interpreter.CallBikeFunction(Function,
                                                              Target,
                                                              interpreter.
                                                                  MarshallArgumentsToBike(
                                                                      args));
                    if (ReturnType == typeof(void))
                        return null;
                    result = interpreter.MarshallToClr(result);
                    object adjustedResult = result;
                    if (TryConvert(ReturnType, Target, ref adjustedResult))
                        return adjustedResult;
                    throw ErrorFactory.CreateClrError(string.Format(
                                                   "Invalid return type: expect {0}, actual {1}",
                                                   ReturnType, result));
                };
                return newThread ? InterpreterHelper.ActAndHandleException(func, true) : func();
            }
        }
    }
}
