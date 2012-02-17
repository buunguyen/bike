namespace Bike.Ast
{
    public partial class ForInStatement : Statement
    {
        public VariableDeclaration VariableDeclaration;
        public ExprNode LeftHandSideExpression;
        public ExprNode Collection;
        public Statement Body;
    }
}
