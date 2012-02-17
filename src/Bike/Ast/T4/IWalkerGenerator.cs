
namespace Bike.Ast {
	public interface IWalker {
			
		object Walk(SourceElements node);
			
		object Walk(SourceUnitTree node);
			
		object Walk(FormalParameter node);
			
		object Walk(FunctionDeclaration node);
			
		object Walk(VariableDeclaration node);
			
		object Walk(AdditiveExpression node);
			
		object Walk(AndExpression node);
			
		object Walk(Argument node);
			
		object Walk(Arguments node);
			
		object Walk(CallExpression node);
			
		object Walk(ConditionalExpression node);
			
		object Walk(EqualityExpression node);
			
		object Walk(ExecExpression node);
			
		object Walk(Expression node);
			
		object Walk(FunctionExpression node);
			
		object Walk(Identifier node);
			
		object Walk(IndexSuffix node);
			
		object Walk(LeftAssignmentExpression node);
			
		object Walk(MemberExpression node);
			
		object Walk(MultiplicativeExpression node);
			
		object Walk(OrExpression node);
			
		object Walk(PropertyReferenceSuffix node);
			
		object Walk(RelationalExpression node);
			
		object Walk(SelfExpression node);
			
		object Walk(TypeDescriptor node);
			
		object Walk(TypeDescriptorSuffix node);
			
		object Walk(UnaryExpression node);
			
		object Walk(ArrayLiteral node);
			
		object Walk(ObjectLiteral node);
			
		object Walk(PrimitiveLiteral node);
			
		object Walk(BreakStatement node);
			
		object Walk(CaseClause node);
			
		object Walk(EmptyStatement node);
			
		object Walk(ExpressionStatement node);
			
		object Walk(ForInStatement node);
			
		object Walk(IfStatement node);
			
		object Walk(LoadStatement node);
			
		object Walk(NextStatement node);
			
		object Walk(RescueClause node);
			
		object Walk(ReturnStatement node);
			
		object Walk(StatementBlock node);
			
		object Walk(SwitchStatement node);
			
		object Walk(ThrowStatement node);
			
		object Walk(TryStatement node);
			
		object Walk(VariableStatement node);
			
		object Walk(WhileStatement node);
			}
}