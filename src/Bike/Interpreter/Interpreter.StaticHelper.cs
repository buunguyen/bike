namespace Bike.Interpreter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ast;
    using Builtin;
    using Parser;

    internal static class InterpreterHelper
    {
        public const string SpecialSuffix = "__";

        public static object ActAndHandleException(Func<object> func, bool onThread = false)
        {
            try
            {
                try
                {
                    return func();
                }
                catch (Exception e)
                {
                    if (e is Break || e is Next)
                        // treat like null return;
                        return null; 
                    if (e is ParseException || e is BikeObject)
                        throw;
                    throw ErrorFactory.CreateClrError(e);
                }
            } 
            catch (Exception e)
            {
                if (!onThread)
                    throw;
                Console.WriteLine(InterpretationContext.Instance.
                    Interpreter.Stringify(e, true));
                return null;
            }
        }
        
        public static bool IsTrue(object cond)
        {
            return (cond is BikeBoolean && ((BikeBoolean)cond).Value)
                   || (!(cond is BikeBoolean) && cond != null);
        }

        public static string SuffixValue(this Node suffix, Interpreter interpreter)
        {
            var prop = suffix as PropertyReferenceSuffix;
            return prop != null
                ? prop.Identifier.Value
                : CheckIndex(suffix.Accept(interpreter), interpreter);
        }

        public static string CheckIndex(object index, Interpreter interpreter)
        {
            if (!(index is BikeString) && !(index is BikeNumber))
                throw ErrorFactory.CreateTypeError(string.Format("Index {0} is not evaluated to string or number", index));
            if (index is BikeString)
                return ((BikeString)index).Value;
            return ((BikeNumber)index).Value.ToString();
        }

        public static T As<T>(this Node node, string expectedType) where T : Node
        {
            if (node.GetType() == typeof(T))
            {
                return (T) node;
            }
            throw new InvalidProgramException(string.Format("{0} is expected", expectedType));
        }

        public static object[] Arguments(this Node suffix, Interpreter interpreter)
        {
            var indexSuffix = (IndexSuffix)suffix;
            return indexSuffix.Expression is Expression
                       ? ((Expression) indexSuffix.Expression).AssignmentExpressions
                            .Select(e => e.Accept(interpreter)).ToArray()
                       : new[] {indexSuffix.Accept(interpreter)};
        }

        public static List<Node> ExceptLast(this List<Node> suffixes)
        {
            var result = new List<Node>();
            for (int i = 0; i < suffixes.Count - 1; i++)
            {
                result.Add(suffixes[i]);
            }
            return result;
        }

        public static List<Node> ExceptFirst(this List<Node> suffixes)
        {
            var result = new List<Node>();
            for (int i = 1; i < suffixes.Count; i++)
            {
                result.Add(suffixes[i]);
            }
            return result;
        }
    }
}
