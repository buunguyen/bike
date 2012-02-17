namespace Bike.Ast
{
    using Parser;

    public partial class RelationalExpression : ExprNode
    {
        public ExprNode LeftExpression;
        public TokenType Operator;
        public ExprNode RightExpression;
    }
}
