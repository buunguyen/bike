namespace Bike.Ast
{
    using System.Collections.Generic;

    public partial class ObjectLiteral : ExprNode
    {
        public readonly Dictionary<Node, ExprNode> Properties =
            new Dictionary<Node, ExprNode>();
    }
}
