namespace Bike.Ast
{
    using System.Collections.Generic;

    public partial class Expression : ExprNode
    {
        public readonly List<ExprNode> AssignmentExpressions = new List<ExprNode>();
    }
}
