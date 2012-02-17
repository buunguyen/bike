namespace Bike.Ast
{
    using System.Collections.Generic;

    public partial class SwitchStatement : Statement
    {
        public ExprNode Condition;
        public readonly List<CaseClause> Cases = new List<CaseClause>();
    }
}
