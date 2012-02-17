namespace Bike.Ast
{
    public partial class TryStatement : Statement
    {
        public StatementBlock Body;
        public RescueClause Rescue;
        public StatementBlock Finally;
    }
}
