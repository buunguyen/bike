using Fasterflect;

namespace Bike.Interpreter
{
    using System;
    using Ast;
    using Builtin;

    public partial class Interpreter
    {
        public override object Walk(LeftAssignmentExpression node)
        {
            // Identifier, resolve from scope
            if (node.Variable is Identifier)
            {
                var scope = Context.CurrentFrame;
                var var = ((Identifier)node.Variable);
                var suffix = new PropertyReferenceSuffix { Identifier = var };
                var lvThunk = (Func<object>)(() => var.Accept(this));
                var rvThunk = (Func<object>)(() => node.Operand.Accept(this));
                return AssignSwitch(node.Operator.Text, scope, suffix, lvThunk, rvThunk);
            }
            else
            {
                var suffixes = node.Variable is MemberExpression
                                           ? ((MemberExpression)node.Variable).Suffixes
                                           : ((CallExpression)node.Variable).Suffixes;
                var secondLastObject = node.Variable is MemberExpression
                                           ? Member((MemberExpression)node.Variable, suffixes.ExceptLast())
                                           : Call((CallExpression)node.Variable, suffixes.ExceptLast());
                var lvThunk = (Func<object>)(() =>
                    node.Variable is MemberExpression
                                           ? Member((MemberExpression)node.Variable, suffixes)
                                           : Call((CallExpression)node.Variable, suffixes));
                var rvThunk = (Func<object>) (() => node.Operand.Accept(this));
                var suffix = suffixes[suffixes.Count - 1];

                // Static .NET, Console.Out = ...)
                if (secondLastObject is Type)
                {
                    return AssignSwitch(node.Operator.Text, secondLastObject, suffix, lvThunk, rvThunk, true);
                }
                return AssignSwitch(node.Operator.Text, secondLastObject, suffix, lvThunk, rvThunk);
            }
        }

        private object AssignSwitch(string @operator, object target, Node suffix,
                                    Func<object> lvThunk, Func<object> rvThunk,
                                    bool staticDotNetAssign = false)
        {
            switch (@operator)
            {
                case "=":
                    {
                        return PerformAssign(target, suffix, rvThunk(), staticDotNetAssign);
                    }
                case "*=":
                    {
                        var result = OpMultiply(lvThunk(), rvThunk());
                        return PerformAssign(target, suffix, result, staticDotNetAssign);
                    }
                case "/=":
                    {
                        var result = OpDivide(lvThunk(), rvThunk());
                        return PerformAssign(target, suffix, result, staticDotNetAssign);
                    }
                case "%=":
                    {
                        var result = Modulus(lvThunk(), rvThunk());
                        return PerformAssign(target, suffix, result, staticDotNetAssign);
                    }
                case "+=":
                    {
                        var rv = rvThunk();
                        if (rv is BikeFunction)
                        {
                            var fieldName = suffix.SuffixValue(this);
                            AddHandler(target, fieldName, (BikeFunction)rv, this, staticDotNetAssign);
                            var eventInfo = staticDotNetAssign
                                ? ((Type)target).GetEvent(fieldName)
                                : target.GetType().GetEvent(fieldName);
                            // if delegate, return its value; if event, return null
                            return eventInfo != null ? null : lvThunk();
                        }
                        var result = OpAdd(lvThunk(), rv);
                        return PerformAssign(target, suffix, result, staticDotNetAssign);
                    }
                case "-=":
                    {
                        var result = OpMinus(lvThunk(), rvThunk());
                        return PerformAssign(target, suffix, result, staticDotNetAssign);
                    }
                case "||=":
                    {
                        var result = OpOr(lvThunk(), rvThunk);
                        return PerformAssign(target, suffix, result, staticDotNetAssign);
                    }
                case "&&=":
                    {
                        var result = OpAnd(lvThunk(), rvThunk);
                        return PerformAssign(target, suffix, result, staticDotNetAssign);
                    }
                default:
                    throw ErrorFactory.CreateInvalidProgramError(string.Format("Invalid operator {0}", @operator));
            }
        }

        private object PerformAssign(object target, Node suffix, object value,
                                            bool staticDotNetAssign)
        {
            if (staticDotNetAssign)
            {
                SetStaticProperty((Type)target, suffix.SuffixValue(this), value);
            }
            else
            {
                var scope = target as IScope;
                if (scope != null)
                {
                    scope.Assign(suffix.SuffixValue(this), value);
                }
                else if (suffix is IndexSuffix)
                {
                    SetInstanceIndexer(target, suffix.Arguments(this), value);
                }
                else
                {
                    SetInstanceProperty(target, suffix.SuffixValue(this), value);
                }
            }
            return value;
        }
    }
}
