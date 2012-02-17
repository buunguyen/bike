namespace Bike.Ast
{
    using Parser;

    public partial class UnaryExpression : ExprNode
    {
        public TokenType Prefix;
        public TokenType Postfix;
        public ExprNode Expression;
    }
}
