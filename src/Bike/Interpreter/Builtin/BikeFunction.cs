namespace Bike.Interpreter.Builtin
{
    using System.Collections.Generic;
    using System.Text;
    using Ast;
    using Bike.Interpreter;

    public class BikeFunction : BikeObject
    {
        public readonly Node Node;
        private volatile string description;
        public readonly List<FormalParameter> Parameters;
        public readonly SourceElements Body;
        public readonly ScopeFrame BoundScope;
        public const string Anonymous = "<Anonymous>";
        
        public BikeString Name
        {
            get { return (BikeString)Members["name"]; }
            set { Members["name"] = value; }
        }

        public BikeFunction(Node node, Identifier funcName,
            List<FormalParameter> parameters, SourceElements body,
            ScopeFrame boundScope) : base(InterpretationContext.FunctionBase)
        {
            Node = node;
            Name = new BikeString(funcName == null ? Anonymous : funcName.Value);
            Parameters = parameters;
            Body = body;
            BoundScope = boundScope;
            DefineVariables();
        }

        public string Description
        {
            get
            {
                if (description == null)
                {
                    var sb = new StringBuilder(Name.Value);
                    if (Parameters.Count == 0)
                        return sb.Append("()").ToString();
                    sb.Append("(");
                    foreach (var parameter in Parameters)
                    {
                        sb.Append(parameter.IsParams ? "*" : "")
                            .Append(parameter.Identifier.Value).Append(", ");
                    }
                    var value = sb.ToString();
                    description = value.Substring(0, value.Length - 2) + ")";
                }
                return description;
            }
        }

        private void DefineVariables()
        {
            var paramsObj = new BikeArray();
            for (int i = 0; i < Parameters.Count; i++)
            {
                var parameter = Parameters[i];
                var paramObj = new BikeObject(InterpretationContext.ObjectBase);
                paramObj.Define("name", new BikeString(parameter.Identifier.Value));
                paramObj.Define("is_params", new BikeBoolean(parameter.IsParams));
                paramsObj.Define(i.ToString(), paramObj);
            }
            Members["params"] = paramsObj;
        }
    }
}
