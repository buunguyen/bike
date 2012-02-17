namespace Bike.Ast
{
    using System.Collections.Generic;

    public partial class CallExpression : ExprNode
    {
        public ExprNode Member;
        public Arguments Arguments;
        public readonly List<Node> Suffixes = new List<Node>();
    }
}
