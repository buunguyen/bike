namespace Bike.Ast
{
    using Parser;

    public partial class LeftAssignmentExpression : ExprNode
    {
        public ExprNode Variable;
        public Token Operator;
        public ExprNode Operand;
    }
}
