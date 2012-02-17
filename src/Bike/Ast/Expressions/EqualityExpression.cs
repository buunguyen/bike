namespace Bike.Ast
{
    using Parser;

    public partial class EqualityExpression : ExprNode
    {
        public ExprNode LeftExpression;
        public TokenType Operator;
        public ExprNode RightExpression;
    }
}
