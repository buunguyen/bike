namespace Bike.Ast
{
    public partial class IfStatement : Statement
    {
        public ExprNode Condition;
        public Statement Body;
        public Statement Else;
    }
}
