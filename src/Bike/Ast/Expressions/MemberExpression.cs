namespace Bike.Ast
{
    using System.Collections.Generic;

    public partial class MemberExpression : ExprNode
    {
        public ExprNode Expression;
        public List<Node> Suffixes;
    }
}
