namespace Bike.Test
{
    using System;
    using Ast;

    public class PrintNodeWalker : NodeWalker
    {
        private int level = 0;

        private void Print(string str, params object[] args)
        {
            for (int i = 0; i < level; i++)
                Console.Write("  ");
            Console.WriteLine(str, args);
        }

        public override bool Enter(SourceElements node)
        {
            Print("SourceElements");
            level++;
            return true;
        }

        public override void Exit(SourceElements node)
        {
            level--;
        }

        public override bool Enter(SourceUnitTree node)
        {
            Print("********************* Code Tree *********************");
            return true;
        }

        public override void Exit(SourceUnitTree node)
        {
        }

        public override bool Enter(FunctionDeclaration node)
        {
            Print("FunctionDeclaration");
            level++;
            return true;
        }

        public override void Exit(FunctionDeclaration node)
        {
            level--;
        }

        public override bool Enter(ExecExpression node)
        {
            Print("EvalExpression");
            level++;
            return true;
        }

        public override void Exit(ExecExpression node)
        {
            level--;
        }

        public override bool Enter(VariableDeclaration node)
        {
            Print("VariableDeclaration");
            level++;
            return true;
        }

        public override void Exit(VariableDeclaration node)
        {
            level--;
        }

        public override bool Enter(AdditiveExpression node)
        {
            Print("AdditiveExpression '{0}'", node.Operator);
            level++;
            return true;
        }

        public override void Exit(AdditiveExpression node)
        {
            level--;
        }

        public override bool Enter(AndExpression node)
        {
            Print("AndExpression");
            level++;
            return true;
        }

        public override void Exit(AndExpression node)
        {
            level--;
        }

        public override bool Enter(Arguments node)
        {
            Print("Arguments");
            level++;
            return true;
        }

        public override void Exit(Arguments node)
        {
            level--;
        }

        public override bool Enter(Argument node)
        {
            Print("Argument");
            level++;
            return true;
        }

        public override void Exit(Argument node)
        {
            level--;
        }

        public override bool Enter(CallExpression node)
        {
            Print("CallExpression");
            level++;
            return true;
        }

        public override void Exit(CallExpression node)
        {
            level--;
        }

        public override bool Enter(ConditionalExpression node)
        {
            Print("ConditionalExpression");
            level++;
            return true;
        }

        public override void Exit(ConditionalExpression node)
        {
            level--;
        }

        public override bool Enter(EqualityExpression node)
        {
            Print("EqualityExpression");
            level++;
            return true;
        }

        public override void Exit(EqualityExpression node)
        {
            level--;
        }

        public override bool Enter(Expression node)
        {
            Print("Expression");
            level++;
            return true;
        }

        public override void Exit(Expression node)
        {
            level--;
        }

        public override bool Enter(FunctionExpression node)
        {
            Print("FunctionExpression");
            level++;
            return true;
        }

        public override void Exit(FunctionExpression node)
        {
            level--;
        }

        public override bool Enter(Identifier node)
        {
            Print("Identifier '{0}'", node.Value);
            level++;
            return true;
        }

        public override void Exit(Identifier node)
        {
            level--;
        }

        public override bool Enter(IndexSuffix node)
        {
            Print("IndexSuffix");
            level++;
            return true;
        }

        public override void Exit(IndexSuffix node)
        {
            level--;
        }

        public override bool Enter(LeftAssignmentExpression node)
        {
            Print("LeftAssignmentExpression {0}", node.Operator);
            level++;
            return true;
        }

        public override void Exit(LeftAssignmentExpression node)
        {
            level--;
        }

        public override bool Enter(MemberExpression node)
        {
            Print("MemberExpression");
            level++;
            return true;
        }

        public override void Exit(MemberExpression node)
        {
            level--;
        }

        public override bool Enter(MultiplicativeExpression node)
        {
            Print("MultiplicativeExpression ''{0}''", node.Operator);
            level++;
            return true;
        }

        public override void Exit(MultiplicativeExpression node)
        {
            level--;
        }

        public override bool Enter(TypeDescriptorSuffix node)
        {
            Print("TypeDescriptorSuffix");
            level++;
            return true;
        }

        public override void Exit(TypeDescriptorSuffix node)
        {
            level--;
        }

        public override bool Enter(TypeDescriptor node)
        {
            Print("TypeDescriptor: {0}", node.Name);
            level++;
            return true;
        }

        public override void Exit(TypeDescriptor node)
        {
            level--;
        }

        public override bool Enter(OrExpression node)
        {
            Print("OrExpression");
            level++;
            return true;
        }

        public override void Exit(OrExpression node)
        {
            level--;
        }

        public override bool Enter(PropertyReferenceSuffix node)
        {
            Print("PropertyReferenceSuffix");
            level++;
            return true;
        }

        public override void Exit(PropertyReferenceSuffix node)
        {
            level--;
        }

        public override bool Enter(RelationalExpression node)
        {
            Print("RelationalExpression");
            level++;
            return true;
        }

        public override void Exit(RelationalExpression node)
        {
            level--;
        }

        public override bool Enter(SelfExpression node)
        {
            Print("SelfExpression");
            level++;
            return true;
        }

        public override void Exit(SelfExpression node)
        {
            level--;
        }

        public override bool Enter(UnaryExpression node)
        {
            Print("UnaryExpression Prefix: '{0}', Postfix: '{1}'",
                node.Prefix, node.Postfix);
            level++;
            return true;
        }

        public override void Exit(UnaryExpression node)
        {
            level--;
        }

        public override bool Enter(ArrayLiteral node)
        {
            Print("ArrayLiteral");
            level++;
            return true;
        }

        public override void Exit(ArrayLiteral node)
        {
            level--;
        }

        public override bool Enter(ObjectLiteral node)
        {
            Print("ObjectLiteral");
            level++;
            return true;
        }

        public override void Exit(ObjectLiteral node)
        {
            level--;
        }

        public override bool Enter(PrimitiveLiteral node)
        {
            Print("PrimitiveLiteral '{0}': '{1}'", node.Type, node.Value);
            level++;
            return true;
        }

        public override void Exit(PrimitiveLiteral node)
        {
            level--;
        }

        public override bool Enter(BreakStatement node)
        {
            Print("BreakStatement");
            level++;
            return true;
        }

        public override void Exit(BreakStatement node)
        {
            level--;
        }

        public override bool Enter(CaseClause node)
        {
            Print("CaseClause");
            level++;
            return true;
        }

        public override void Exit(CaseClause node)
        {
            level--;
        }

        public override bool Enter(EmptyStatement node)
        {
            Print("EmptyStatement");
            level++;
            return true;
        }

        public override void Exit(EmptyStatement node)
        {
            level--;
        }

        public override bool Enter(ExpressionStatement node)
        {
            Print("ExpressionStatement");
            level++;
            return true;
        }

        public override void Exit(ExpressionStatement node)
        {
            level--;
        }

        public override bool Enter(ForInStatement node)
        {
            Print("ForInStatement");
            level++;
            return true;
        }

        public override void Exit(ForInStatement node)
        {
            level--;
        }

        public override bool Enter(IfStatement node)
        {
            Print("IfStatement");
            level++;
            return true;
        }

        public override void Exit(IfStatement node)
        {
            level--;
        }

        public override bool Enter(LoadStatement node)
        {
            Print("ReferenceStatement");
            level++;
            return true;
        }

        public override void Exit(LoadStatement node)
        {
            level--;
        }

        public override bool Enter(NextStatement node)
        {
            Print("NextStatement");
            level++;
            return true;
        }

        public override void Exit(NextStatement node)
        {
            level--;
        }

        public override bool Enter(RescueClause node)
        {
            Print("RescueClause");
            level++;
            return true;
        }

        public override void Exit(RescueClause node)
        {
            level--;
        }

        public override bool Enter(ReturnStatement node)
        {
            Print("ReturnStatement");
            level++;
            return true;
        }

        public override void Exit(ReturnStatement node)
        {
            level--;
        }

        public override bool Enter(StatementBlock node)
        {
            Print("StatementBlock");
            level++;
            return true;
        }

        public override void Exit(StatementBlock node)
        {
            level--;
        }

        public override bool Enter(SwitchStatement node)
        {
            Print("SwitchStatement");
            level++;
            return true;
        }

        public override void Exit(SwitchStatement node)
        {
            level--;
        }

        public override bool Enter(ThrowStatement node)
        {
            Print("ThrowStatement");
            level++;
            return true;
        }

        public override void Exit(ThrowStatement node)
        {
            level--;
        }

        public override bool Enter(TryStatement node)
        {
            Print("TryStatement");
            level++;
            return true;
        }

        public override void Exit(TryStatement node)
        {
            level--;
        }

        public override bool Enter(VariableStatement node)
        {
            Print("VariableStatement");
            level++;
            return true;
        }

        public override void Exit(VariableStatement node)
        {
            level--;
        }

        public override bool Enter(WhileStatement node)
        {
            Print("WhileStatement");
            level++;
            return true;
        }

        public override void Exit(WhileStatement node)
        {
            level--;
        }
    }
}
