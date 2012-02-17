namespace Bike.Ast
{
    using Parser;

    public partial class PrimitiveLiteral : ExprNode
    {
        public string Value;
        public TokenType Type;
    }
}
