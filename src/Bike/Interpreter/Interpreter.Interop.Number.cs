namespace Bike.Interpreter
{
    using System;
    using System.Collections.Generic;

	public partial class Interpreter
	{
        private static readonly IDictionary<Type, decimal[]> WholeNumberRanges = new Dictionary<Type, decimal[]>
            {
                { typeof (byte), new decimal[] {byte.MinValue, byte.MaxValue} },
                { typeof (sbyte), new decimal[] {sbyte.MinValue, sbyte.MaxValue} },
                { typeof (short), new decimal[] {short.MinValue, short.MaxValue} },
                { typeof (ushort), new decimal[] {ushort.MinValue, ushort.MaxValue} },
                { typeof (int), new decimal[] {int.MinValue, int.MaxValue} },
                { typeof (uint), new decimal[] {uint.MinValue, uint.MaxValue} },
                { typeof (long), new decimal[] {long.MinValue, long.MaxValue} },
                { typeof (ulong), new decimal[] {ulong.MinValue, ulong.MaxValue} }
            };
    
        private static bool IsInRange(decimal number, Type targetNumberType)
        {
            if (targetNumberType == typeof(decimal))
            {
                return true;
            }
            if (targetNumberType == typeof(float))
            {
                float f;
                return float.TryParse(number.ToString(), out f);
            }
            if (targetNumberType == typeof(double))
            {
                double d;
                return double.TryParse(number.ToString(), out d);
            }
            
            // Must be whole number type
            var wholePart = Math.Truncate(number);
            if (wholePart != number)
                return false;
            var ranges = WholeNumberRanges[targetNumberType];
            return ranges[0] <= wholePart && wholePart <= ranges[1];
        }

        private static readonly IList<Type> NumberTypes = new List<Type> 
        {
                typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
                typeof(int), typeof(uint), typeof(long), typeof(ulong),
                typeof(float), typeof(double), typeof(decimal)
        };

        private static bool IsNumber(Type type)
        {
            return type != null && NumberTypes.Contains(type);
        }

        private static bool IsNumber(object dotNetObject)
        {
            return dotNetObject != null && IsNumber(dotNetObject.GetType());
        }
	}
}
