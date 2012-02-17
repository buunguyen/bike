namespace Bike.Test
{
    using System;
    using Interpreter;
    using Interpreter.Builtin;
    using NUnit.Framework;

    static class Extensions
    {
        public static InterpretationContext Equal(this InterpretationContext context, 
            string var, object expected)
        {
            object actual = Resolve(var, context.GlobalFrame);
            Assert.AreEqual(expected, actual, string.Format("Variable {0}", var));
            return context;
        }

        public static InterpretationContext Null(this InterpretationContext context, 
            string var)
        {
            var actual = Resolve(var, context.GlobalFrame);
            Assert.IsNull(actual, string.Format("Variable {0}", var));
            return context;
        }

        public static InterpretationContext NotNull(this InterpretationContext context, 
            string var)
        {
            var actual = Resolve(var, context.GlobalFrame);
            Assert.IsNotNull(actual, string.Format("Variable {0}", var));
            return context;
        }

        public static InterpretationContext NotExist(this InterpretationContext context, 
            string var)
        {
            try
            {
                context.GlobalFrame.Resolve(var);
                Assert.Fail();
            } 
            catch (BikeObject bo)
            {
				if (!ErrorFactory.IsNotDefinedError(bo))
					throw;
            }
            return context;
        }

        private static object Resolve(string var, ScopeFrame scopeFrame)
        {
            object actual;
            if (var.Contains("."))
            {
                var parts = var.Split('.');
                actual = scopeFrame.Resolve(parts[0]);
                for (int i = 1; i < parts.Length; i++)
                {
                    actual = ((IScope)actual).Resolve(parts[i]);
                }
            }
            else
            {
                actual = scopeFrame.Resolve(var);
            }
            return ToClrType(actual);
        }

        private static object ToClrType(object obj)
        {
            if (obj is BikeNumber)
                return ((BikeNumber) obj).Value;
            if (obj is BikeString)
                return ((BikeString)obj).Value;
            if (obj is BikeBoolean)
                return ((BikeBoolean)obj).Value;
            if (obj is char)
                return obj.ToString();
            return obj;
        }
    }
}
