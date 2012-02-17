namespace Bike.Ast
{
    public partial class ConditionalExpression : ExprNode
    {
        public ExprNode Condition;
        public ExprNode TrueExpression;
        public ExprNode FalseExpression;
    }
}
