namespace Bike.Interpreter
{
    using System;
    using System.Linq;
    using Builtin;
    using Fasterflect;

    public partial class Interpreter
    {
        private static readonly Flags StaticFlags = Flags.Static | Flags.Public;
        private static readonly Flags InstanceFlags = Flags.Instance | Flags.Public;
        private const string IndexerSetterName = "set_Item";
        private const string IndexerGetterName = "get_Item";

        private delegate bool CallInterceptors(object target, string funcName, object[] args, out object result);
        private static readonly CallInterceptors[] Intercepters = 
        {
            InterceptObjectPassThrough,
            InterceptArrayIndex,
            InterceptStringIndexGetter,
        };

        /// <summary>
        /// This interception is to make sure arguments are passed
        /// as-is, instead of being converted to CLR equivalents.
        /// </summary>
        private static bool InterceptObjectPassThrough(object target, string funcName, object[] args, out object result)
        {
            if (target is Type)
            {
                if ((Type)target == typeof(System.Runtime.CompilerServices.RuntimeHelpers))
                {
                    if (funcName == "Equals")
                    {
                        if (args == null || args.Length != 2)
                            throw ErrorFactory.CreateClrError("Equals() expect 2 arguments");
						
						// Forwards to RuntimeHelpers.Equals() doesn't work under Mac OSX's Mono
						result = args[0] == args[1];
                        return true;
                    }
                    if (funcName == "GetHashCode")
                    {
                        if (args == null || args.Length != 1)
                            throw ErrorFactory.CreateClrError("GetHashCode() expect 1 argument");
                        result = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(args[0]);
                        return true;
                    }
                }
                else if ((Type)target == typeof(System.Threading.Monitor))
                {
                    if (funcName == "Enter" || funcName == "Exit" || 
                        funcName == "Wait" || funcName == "Pulse" || funcName == "PulseAll")
                    {
                        if (args == null || args.Length != 1)
                            throw ErrorFactory.CreateClrError(string.Format("Monitor.{0}() expect 1 argument", funcName));
                        result = typeof(System.Threading.Monitor).CallMethod(funcName, args[0]);
                        return true;
                    }
                    if (funcName == "TryEnter")
                    {
                        if (args == null || (args.Length < 1 || args.Length > 2))
                            throw ErrorFactory.CreateClrError(string.Format("Monitor.{0}() expect 1 or 2 arguments", funcName));
                        if (args.Length == 1)
                            result = System.Threading.Monitor.TryEnter(args[0]);
                        else
                            result = System.Threading.Monitor.TryEnter(args[0], 
                                (int)(decimal)InterpretationContext.Instance.Interpreter
                                .MarshallToClr(args[1]));
                        return true;
                    }
                }
            }
            result = null;
            return false;
        }

        private static bool InterceptArrayIndex(object target, string funcName, object[] args, out object result)
        {
            if (target is Array)
            {
                if (funcName == IndexerGetterName)
                {
                    if (args.Length != 1)
                        throw ErrorFactory.CreateClrError("Invalid number of indexer arguments");
                    int index = args[0] is BikeNumber
                                    ? (int) ((BikeNumber) args[0]).Value
                                    : (int) (decimal) args[0];
                    result = ((Array)target).GetValue(index);
                    return true;
                }
                if (funcName == IndexerSetterName)
                {
                    if (args.Length != 2)
                        throw ErrorFactory.CreateClrError("Invalid number of indexer arguments");
                    int index = args[0] is BikeNumber
                                    ? (int)((BikeNumber)args[0]).Value
                                    : (int)(decimal)args[0];
                    object value = InterpretationContext.Instance.Interpreter.MarshallToClr(args[1]);
                    ((Array)target).SetValue(value, index);
                    result = null;
                    return true;
                }
            }
            result = null;
            return false;
        }

        private static bool InterceptStringIndexGetter(object target, string funcName, object[] args, out object result)
        {
            if (target is string && funcName == IndexerGetterName)
            {
                if (args.Length != 1)
                    throw ErrorFactory.CreateClrError("Invalid number of indexer arguments");
                int index = args[0] is BikeNumber
                                ? (int)((BikeNumber)args[0]).Value
                                : (int)(decimal)args[0];
                result = ((string)target)[index];
                return true;
            }
            result = null;
            return false;
        }

        private object PerformInvocation(object target, string funcName, object[] args, 
            params Func<object[], object>[] invokers)
        {
            foreach (var interceptor in Intercepters)
            {
                object result;
                if (interceptor(target, funcName, args, out result))
                {
                    return MarshallToBike(result);
                }
            }

            args = MarshallArgumentsToClr(args);
            for (int i = 0; i < invokers.Length - 1; i++)
            {
                // First call must be Fasterflect call
                if (i == 0 && args.Any(arg => arg == null))
                    continue;
                try
                {
                    return MarshallToBike(invokers[i](args));
                }
                catch
                {
                    continue;
                }
            }
            return MarshallToBike(invokers[invokers.Length - 1](args));
        }

        public object CreateInstance(Type type, object[] args)
        {
            return PerformInvocation(type, "ctor", args,
                          innerArgs => type.CreateInstance(InstanceFlags, innerArgs),
                          innerArgs => type.TryCreateInstanceWithValues(TryConvert, InstanceFlags, innerArgs));
        }

        public object GetInstanceIndexer(object target, object[] args)
        {
            return CallInstanceFunction(target, IndexerGetterName, args);
        }

        public void SetInstanceIndexer(object target, object[] args, object value)
        {
            CallInstanceFunction(target, IndexerSetterName, args.Concat(new[] { value }).ToArray());
        }

        public object CallDelegate(Delegate currentTarget, object[] args)
        {
            return CallInstanceFunction(currentTarget, "Invoke", args);
        }

        public object CallInstanceFunction(object target, Type[] typeParams, string funcName, object[] args)
        {
            return PerformInvocation(target, funcName, args,
                          innerArgs => target.WrapIfValueType().CallMethod(typeParams, funcName, InstanceFlags, innerArgs),
                          innerArgs => target.TryCallMethodWithValues(TryConvert, funcName, typeParams, InstanceFlags, innerArgs));
        }

        public object CallInstanceFunction(object target, string funcName, object[] args)
        {
            return PerformInvocation(target, funcName, args,
                          innerArgs => target.WrapIfValueType().CallMethod(funcName, InstanceFlags, innerArgs),
                          innerArgs => target.TryCallMethodWithValues(TryConvert, funcName, InstanceFlags, innerArgs));
        }

        public object CallStaticFunction(Type type, Type[] typeParams, string funcName, object[] args)
        {
            return PerformInvocation(type, funcName, args,
                          innerArgs => type.CallMethod(typeParams, funcName, StaticFlags, innerArgs),
                          innerArgs => type.TryCallMethodWithValues(TryConvert, funcName, typeParams, StaticFlags, innerArgs));
        }

        public object CallStaticFunction(Type type, string funcName, object[] args)
        {
            return PerformInvocation(type, funcName, args,
                          innerArgs => type.CallMethod(funcName, StaticFlags, innerArgs),
                          innerArgs => type.TryCallMethodWithValues(TryConvert, funcName, StaticFlags, innerArgs),
                          innerArgs => type.GetType().GetMethod(funcName).Invoke(type, args));
        }

        public object GetInstanceProperty(object target, string propName)
        {
            if (char.IsLower(propName[0]))
            {
				bool exist;
				var result = TryGetInstanceField(target, propName, out exist);
				if (exist)
					return result;
				result = TryGetInstanceProperty(target, propName, out exist);
				if (exist)
					return result;
            }
            else
            {
				bool exist;
				var result = TryGetInstanceProperty(target, propName, out exist);
				if (exist)
					return result;
				result = TryGetInstanceField(target, propName, out exist);
				if (exist)
					return result;
            }
            throw ErrorFactory.CreateClrError(string.Format("Cannot resolve field or property {0} of type {1}", propName, target.GetType()));
        }

        public void SetInstanceProperty(object target, string propName, object value)
        {
            value = MarshallToClr(value);
            Func<bool> setProp = () =>
            {
                var prop = target.GetType().Property(propName, InstanceFlags);
                if (prop != null)
                {
                    var res = value;
                    if (TryConvert(prop.PropertyType, target, ref res))
                    {
                        prop.SetValue(target, res, null);
                        return true;
                    }
                }
                return false;
            };
            Func<bool> setField = () =>
            {
                var field = target.GetType().Field(propName, InstanceFlags);
                if (field != null)
                {
                    var res = value;
                    if (TryConvert(field.FieldType, target, ref res))
                    {
                        field.SetValue(target, res);
                        return true;
                    }
                }
                return false;
            };
            if (char.IsLower(propName[0]))
            {
                if (setField() || setProp())
                    return;
            }
            else
            {
                if (setProp() || setField())
                    return;
            }
            throw ErrorFactory.CreateClrError(string.Format("Cannot resolve field or property {0} of type {1}", propName, target.GetType()));
        }
		
		private object TryGetInstanceProperty(object target, string propName, out bool exist)
		{
			var prop = target.GetType().GetProperty(propName);
			if (prop == null)
			{		
				exist = false;
				return null;
			}
			exist = true;
            var value = prop.GetValue(target, null);
            return MarshallToBike(value);
		}
		
		private object TryGetInstanceField(object target, string fieldName, out bool exist)
		{
			var field = target.GetType().GetField(fieldName);
			if (field == null)
			{		
				exist = false;
				return null;
			}
			exist = true;
            var value = field.GetValue(target);
            return MarshallToBike(value);
		}
		
		private object TryGetStaticProperty(Type type, string propName, out bool exist)
		{
			var prop = type.GetProperty(propName);
			if (prop == null)
			{		
				exist = false;
				return null;
			}
			exist = true;
            var value = prop.Get();
            return MarshallToBike(value);
		}
		
		private object TryGetStaticField(Type type, string fieldName, out bool exist)
		{
            var field = type.GetField(fieldName);
            if (field == null)
			{
				exist = false;
                return null; 
			}
			exist = true;
			return MarshallToBike(field.IsLiteral
                           ? field.GetValue(null) // can't use code-gen to access constant
                           : field.Get());
		}
		
        public object GetStaticProperty(Type type, string propName)
        {
            if (char.IsLower(propName[0]))
            {
				bool exist;
				var result = TryGetStaticField(type, propName, out exist);
				if (exist)
					return result;
				result = TryGetStaticProperty(type, propName, out exist);
				if (exist)
					return result;
            }
            else
            {
				bool exist;
				var result = TryGetStaticProperty(type, propName, out exist);
				if (exist)
					return result;
				result = TryGetStaticField(type, propName, out exist);
				if (exist)
					return result;
            }
			
			bool instancePropExist;
			var instanceResult = TryGetInstanceProperty(type, propName, out instancePropExist);
			if (instancePropExist)
				return instanceResult;
			
			throw ErrorFactory.CreateClrError(string.Format("Cannot resolve static field or property {0} of type {1}", propName, type));
        }

        public void SetStaticProperty(Type type, string propName, object value)
        {
            value = MarshallToClr(value);
            Func<bool> setProp = () =>
            {
                var prop = type.Property(propName, StaticFlags);
                if (prop != null)
                {
                    object res = value;
                    if (TryConvert(prop.PropertyType, type, ref res))
                    {
                        prop.Set(res);
                        return true;
                    }
                }
                return false;
            };
            Func<bool> setField = () =>
            {
                var field = type.Field(propName, StaticFlags);
                if (field != null)
                {
                    object res = value;
                    if (TryConvert(field.FieldType, type, ref res))
                    {
                        field.Set(res);
                        return true;
                    }
                }
                return false;
            };
            if (char.IsLower(propName[0]))
            {
                if (setField() || setProp())
                    return;
            }
            else
            {
                if (setProp() || setField())
                    return;
            }
            
            throw ErrorFactory.CreateClrError(string.Format("Cannot resolve static field or property {0} of type {1}", propName, type));
        }

        public bool Is(object obj, Type type)
        {
            return obj != null && type.IsAssignableFrom(obj.GetType());
        }

        public object[] MarshallArgumentsToClr(object[] bikeObjects)
        {
            var clrObjects = new object[bikeObjects.Length];
            for (int i = 0; i < clrObjects.Length; i++)
            {
                clrObjects[i] = MarshallToClr(bikeObjects[i]);
            }
            return clrObjects;
        }

        public object[] MarshallArgumentsToBike(object[] clrObjects)
        {
            var bikeObjects = new object[clrObjects.Length];
            for (int i = 0; i < bikeObjects.Length; i++)
            {
                bikeObjects[i] = MarshallToBike(clrObjects[i]);
            }
            return bikeObjects;
        }

        public object MarshallToClr(object bikeObject)
        {
            if (bikeObject == null || !(bikeObject is BikeObject))
                return bikeObject;
            if (bikeObject is BikeBoolean)
                return ((BikeBoolean)bikeObject).Value;
            if (bikeObject is BikeString)
            {
                string value = ((BikeString) bikeObject).Value;
                return value.Length == 1 ? (object)value[0] : value;
            }
            if (bikeObject is BikeNumber)
            {
                return ((BikeNumber) bikeObject).Value;
            }
            if (bikeObject is BikeArray)
            {
                var ba = (BikeArray)bikeObject;
                var arr = new object[ba.Value.Count];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = MarshallToClr(ba.Value[i]);
                }
                return arr;
            }
            return bikeObject;
        }

        public object MarshallToBike(object dotNetObject)
        {
            if (dotNetObject == null || dotNetObject is BikeObject)
                return dotNetObject;
            if (dotNetObject is bool)
                return new BikeBoolean((bool)dotNetObject);
            if (dotNetObject is char)
                return new BikeString(dotNetObject.ToString());
            if (dotNetObject is string)
                return new BikeString((string)dotNetObject);
            if (IsNumber(dotNetObject))
                return new BikeNumber(Convert.ToDecimal(dotNetObject));
            if (dotNetObject is Array)
            {
                var ba = new BikeArray();
                var arr = (Array)dotNetObject;
                for (int i = 0; i < arr.Length; i++)
                {
                    ba.Value.Add(MarshallToBike(arr.GetValue(i)));
                }
                return ba;
            }
            return dotNetObject;
        }
    }
}
