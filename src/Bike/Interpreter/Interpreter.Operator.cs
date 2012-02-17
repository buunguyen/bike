namespace Bike.Interpreter
{
    using System;
    using System.Text;
    using Builtin;

    public partial class Interpreter
    {
        public object OpAdd(object lv, object rv)
        {
            if (lv == null && rv == null)
                throw ErrorFactory.CreateTypeError( "Invalid operand type: 'null'");
            if (lv is BikeString)
                return new BikeString(Stringify(lv) + (rv == null ? "null" : Stringify(rv)));
            if (rv is BikeString)
                return new BikeString((lv == null ? "null" : Stringify(lv)) + Stringify(rv));
            if (lv is BikeNumber && rv is BikeNumber)
                return new BikeNumber(((BikeNumber)lv).Value + ((BikeNumber)rv).Value);
            throw ErrorFactory.CreateTypeError(string.Format("Invalid operand type: {0} and {1}", lv, rv));
        }

        public object OpMinus(object lv, object rv)
        {
            if (lv == null || rv == null)
                throw ErrorFactory.CreateTypeError( "Invalid operand type: 'null'");
            if (lv is BikeNumber && rv is BikeNumber)
                return new BikeNumber(((BikeNumber)lv).Value - ((BikeNumber)rv).Value);
            throw ErrorFactory.CreateTypeError(string.Format("Invalid operand type: {0} - {1}", lv, rv));
        }

        public object OpMultiply(object lv, object rv)
        {
            if (lv == null || rv == null)
                throw ErrorFactory.CreateTypeError( "Invalid operand type: 'null'");
            if (lv is BikeNumber && rv is BikeNumber)
                return new BikeNumber(((BikeNumber)lv).Value * ((BikeNumber)rv).Value);
            throw ErrorFactory.CreateTypeError(string.Format("Invalid operand type: {0} * {1}", lv, rv));
        }

        public object OpDivide(object lv, object rv)
        {
            if (lv == null || rv == null)
                throw ErrorFactory.CreateTypeError( "Invalid operand type: 'null'");
            if (lv is BikeNumber && rv is BikeNumber)
                return new BikeNumber(((BikeNumber)lv).Value / ((BikeNumber)rv).Value);
            throw ErrorFactory.CreateTypeError(string.Format("Invalid operand type: {0} / {1}", lv, rv));
        }

        public object Modulus(object lv, object rv)
        {
            if (lv == null || rv == null)
                throw ErrorFactory.CreateTypeError( "Invalid operand type: 'null'");
            if (lv is BikeNumber && rv is BikeNumber)
                return new BikeNumber(((BikeNumber)lv).Value % ((BikeNumber)rv).Value);
            throw ErrorFactory.CreateTypeError(string.Format("Invalid operand type: {0} % {1}", lv, rv));
        }

        public object OpAnd(object lv, Func<object> rvThunk)
        {
            return InterpreterHelper.IsTrue(lv) ? rvThunk() : lv;
        }

        public object OpOr(object lv, Func<object> rvThunk)
        {
            return InterpreterHelper.IsTrue(lv) ? lv : rvThunk();
        }

        public BikeBoolean OpNot(object value)
        {
            return new BikeBoolean(!InterpreterHelper.IsTrue(value));
        }

        public BikeBoolean OpEqual(object lv, object rv)
        {
            if (lv == null)
                return new BikeBoolean(rv == null);
            if (lv is BikeObject)
            {
                var lbo = (BikeObject) lv;
                if (lbo.Exist("=="))
                {
                    var eq = lbo.Resolve("==");
                    if (eq is BikeFunction)
                    {
                        return (BikeBoolean) CallBikeFunction(
                            (BikeFunction)eq, lbo, new[] { rv });
                    }
                    throw ErrorFactory.CreateTypeError("== is not a function");
                }
                return new BikeBoolean(lbo == rv);
            }
            return new BikeBoolean(lv.Equals(rv));
        }

        public int HashCode(object obj)
        {
            if (obj == null)
                throw ErrorFactory.CreateTypeError("Invalid operand type: 'null'");
            if (obj is BikeObject)
            {
                var lbo = (BikeObject)obj;
                if (lbo.Exist("hash_code"))
                {
                    var hc = lbo.Resolve("hash_code");
                    if (hc is BikeFunction)
                    {
                        return (int)((BikeNumber)CallBikeFunction(
                            (BikeFunction)hc, lbo, new object[0])).Value;
                    }
                    throw ErrorFactory.CreateTypeError("hash_code is not a function");
                }
                throw ErrorFactory.CreateNotDefinedError("hash_code");
            }
            return obj.GetHashCode();
        }

        public string Stringify(object obj, bool printError = false)
        {
            if (obj == null)
                throw ErrorFactory.CreateTypeError( "Invalid operand type: 'null'");
            if (obj is BikeObject)
            {
                var sb = new StringBuilder();
                var bo = (BikeObject) obj;
                if (bo.Exist("to_string"))
                {
                    var ts = bo.Resolve("to_string");
                    if (ts is BikeFunction)
                    {
                        var value = CallBikeFunction((BikeFunction) ts, bo, new object[0]);
                        sb.Append(((BikeString) value).Value);
                    }
                    else
                    {
                        throw ErrorFactory.CreateTypeError("to_string is not a function");
                    }
                }
                else
                {
                    throw ErrorFactory.CreateNotDefinedError("to_string");
                }
                if (printError && !bo.Exist(InterpreterHelper.SpecialSuffix + "is_error") && bo.Exist("stack_trace"))
                {
                    var st = ((BikeString)bo.Resolve("stack_trace")).Value;
                    sb.AppendLine().Append("Stack Trace:").AppendLine().Append(st).AppendLine();
                }
                return sb.ToString();
            }
            return obj.ToString();
        }

        public BikeBoolean OpNotEqual(object lv, object rv)
        {
            return OpNot(OpEqual(lv, rv));
        }

        public object OpLessThan(object lv, object rv)
        {
            if (lv is BikeNumber && rv is BikeNumber)
                return new BikeBoolean(((BikeNumber)lv).Value < ((BikeNumber)rv).Value);
            throw ErrorFactory.CreateTypeError(string.Format("Invalid operand type: {0} < {1}", lv, rv));
        }

        public object OpLessThanOrEqual(object lv, object rv)
        {
            if (lv is BikeNumber && rv is BikeNumber)
                return new BikeBoolean(((BikeNumber)lv).Value <= ((BikeNumber)rv).Value);
            throw ErrorFactory.CreateTypeError(string.Format("Invalid operand type: {0} <= {1}", lv, rv));
        }

        public object OpGreaterThan(object lv, object rv)
        {
            if (lv is BikeNumber && rv is BikeNumber)
                return new BikeBoolean(((BikeNumber)lv).Value > ((BikeNumber)rv).Value);
            throw ErrorFactory.CreateTypeError(string.Format("Invalid operand type: {0} > {1}", lv, rv));
        }

        public object OpGreaterThanOrEqual(object lv, object rv)
        {
            if (lv is BikeNumber && rv is BikeNumber)
                return new BikeBoolean(((BikeNumber)lv).Value >= ((BikeNumber)rv).Value);
            throw ErrorFactory.CreateTypeError(string.Format("Invalid operand type: {0} >= {1}", lv, rv));
        }

        public object OpDoublePlus(object value)
        {
            if (value == null)
                throw ErrorFactory.CreateTypeError( "Invalid operand type: 'null'");
            if (value is BikeNumber)
                return new BikeNumber(((BikeNumber)value).Value + 1);
            throw ErrorFactory.CreateTypeError(string.Format("Invalid operand type: {0}++", value));
        }

        public object OpDoubleMinus(object value)
        {
            if (value == null)
                throw ErrorFactory.CreateTypeError( "Invalid operand type: 'null'");
            if (value is BikeNumber)
                return new BikeNumber(((BikeNumber)value).Value - 1);
            throw ErrorFactory.CreateTypeError(string.Format("Invalid operand type: {0}--", value));
        }

        public object OpPlus(object value)
        {
            if (value == null)
                throw ErrorFactory.CreateTypeError( "Invalid operand type: 'null'");
            if (value is BikeNumber)
                return value;
            throw ErrorFactory.CreateTypeError(string.Format("Invalid operand type: +{0}", value));
        }

        public object OpMinus(object value)
        {
            if (value == null)
                throw ErrorFactory.CreateTypeError( "Invalid operand type: 'null'");
            if (value is BikeNumber)
                return new BikeNumber(-((BikeNumber)value).Value);
            throw ErrorFactory.CreateTypeError(string.Format("Invalid operand type: -{0}", value));
        }
	}
}
