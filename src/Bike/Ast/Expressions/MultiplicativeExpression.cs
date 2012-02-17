namespace Bike.Ast
{
    using Parser;

    public partial class MultiplicativeExpression : ExprNode
    {
        public ExprNode LeftExpression;
        public TokenType Operator;
        public ExprNode RightExpression;
    }
}
