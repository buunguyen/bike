namespace Bike.Ast
{
    using System.Collections.Generic;

    public partial class CaseClause : Node
    {
        public ExprNode Expression;
        public List<Statement> Body;
        public bool IsDefault { get { return Expression == null; } }
    }
}
