namespace Bike.Ast
{
    using System.Collections.Generic;

    public partial class ArrayLiteral : ExprNode
    {
        public readonly List<ExprNode> Expressions = new List<ExprNode>();
        public bool IsRange;
    }
}
