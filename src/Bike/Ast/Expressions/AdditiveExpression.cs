namespace Bike.Ast
{
    using Parser;

    public partial class AdditiveExpression : ExprNode
    {
        public ExprNode LeftExpression;
        public TokenType Operator;
        public ExprNode RightExpression;
    }
}
