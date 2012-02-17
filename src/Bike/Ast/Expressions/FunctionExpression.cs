namespace Bike.Ast
{
    using System.Collections.Generic;

    public partial class FunctionExpression : ExprNode
    {
        public Identifier Identifier;
        public List<FormalParameter> FormalParameters;
        public SourceElements Body;
    }
}
