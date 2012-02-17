using System;
using Bike.Interpreter.Builtin;
using Fasterflect;

namespace Bike.Interpreter
{
    public partial class Interpreter
    {
        private static bool TryConvert(Type targetType, object owner, ref object value)
        {
            foreach (var rules in ConversionRules)
            {
                try
                {
                    if (rules.Convert(targetType, owner, ref value))
                        return true;
                }
                catch (ImmediateStop)
                {
                    return false;
                }
            }
            return false;   
        }

        private class TypeCompatibilityRule : IConversionRule
        {
            public bool Convert(Type targetType, object owner, ref object value)
            {
                if (value == null)
                    // todo nullable check
                    return !typeof(ValueType).IsAssignableFrom(targetType);
                var valueType = value.GetType();
                if (targetType.IsAssignableFrom(valueType))
                    return true;
                if (targetType == typeof(string) && !(value is string) && !(value is char))
                    throw new ImmediateStop();
                if (targetType == typeof(char) && !(value is char) && (!(value is string) || ((string)value).Length != 1))
                    throw new ImmediateStop();
                if (targetType == typeof(bool) && !(value is bool))
                    throw new ImmediateStop();
                if (targetType.IsEnum && !IsNumber(valueType))
                    throw new ImmediateStop();
                if (IsNumber(targetType) && 
                        (!IsNumber(valueType) || 
                         !IsInRange((decimal)value, targetType)))
                    throw new ImmediateStop();
                return false; 
            }
        }

        private class FunctionConversionRule : IConversionRule
        {
            public bool Convert(Type targetType, object owner, ref object value)
            {
                if (!(value is BikeFunction) || !typeof(Delegate).IsAssignableFrom(targetType))
                    return false;

                var invoker = new BikeCallback(owner,
                                                targetType.GetMethod("Invoke").ReturnType,
                                                (BikeFunction) value);
                value = targetType.BuildDynamicHandler(invoker.Callback);
                return true;
            }
        }

        private class ArrayConversionRule : IConversionRule
        {
            public bool Convert(Type targetType, object owner, ref object value)
            {
                if (!(value is object[]) || !targetType.IsArray)
                    return false;

                if (targetType == typeof(object[]))
                    return true;

                var values = ((object[])value);
                var array = (Array)targetType.CreateInstance(values.Length);
                var elementType = array.GetType().GetElementType();
                for (int i = 0; i < values.Length; i++)
                {
                    object element = values[i];
                    try
                    {
                        if (TryConvert(elementType, owner, ref element))
                            array.SetValue(element, i);
                        else
                            return false;
                    } 
                    catch (ImmediateStop)
                    {
                        return false;
                    }
                }
                value = array;
                return true;
            }
        }

        private class BuiltinConversionRule : IConversionRule
        {
            public bool Convert(Type targetType, object owner, ref object value)
            {
                if (!(value is IConvertible))
                    return false;                
                try
                {
                    value = System.Convert.ChangeType(value, targetType);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private class ImmediateStop : Exception {}

        private interface IConversionRule
        {
            /// <summary>
            /// Returns true to indicate a conversion has been done successfully.
            /// Returns false to indicate no conversion has been done and should try next.
            /// Throws ImmediateStop if a clear non-match is observed.
            /// </summary>
            /// <returns></returns>
            bool Convert(Type targetType, object owner, ref object value);
        }

        private static readonly IConversionRule[] ConversionRules = 
        {
            new TypeCompatibilityRule(),
            new FunctionConversionRule(),
            new ArrayConversionRule(),
            new BuiltinConversionRule()
        };
    }
}
