namespace Bike.Ast
{
    public partial class WhileStatement : Statement
    {
        public ExprNode Condition;
        public Statement Body;
    }
}
