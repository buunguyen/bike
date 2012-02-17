namespace Bike.Ast
{
    public abstract partial class NodeWalker : IWalker
    {
        public virtual object Walk(SourceUnitTree node)
        {
            if (Enter(node))
            {
                if (node.SourceElements != null)
                    node.SourceElements.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(SourceElements node)
        {
            if (Enter(node))
            {
                foreach (var statement in node.Statements)
                {
                    statement.Accept(this);
                }
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(FunctionDeclaration node)
        {
            if (Enter(node))
            {
                node.Identifier.Accept(this);
                foreach (var parameter in node.FormalParameters)
                {
                    parameter.Accept(this);
                }
                node.Body.Accept(this);
            }
            Exit(node);
            return null;
        }

        public object Walk(FormalParameter node)
        {
            if (Enter(node))
            {
                node.Identifier.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(VariableDeclaration node)
        {
            if (Enter(node))
            {
                node.Identifier.Accept(this);
                if (node.Initializer != null)
                    node.Initializer.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(AdditiveExpression node)
        {
            if (Enter(node))
            {
                node.LeftExpression.Accept(this);
                if (node.RightExpression != null)
                    node.RightExpression.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(AndExpression node)
        {
            if (Enter(node))
            {
                node.LeftExpression.Accept(this);
                if (node.RightExpression != null)
                    node.RightExpression.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(Arguments node)
        {
            if (Enter(node))
            {
                foreach (var exprNode in node.Children)
                {
                    exprNode.Accept(this);
                }
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(Argument node)
        {
            if (Enter(node))
            {
                node.Expression.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(CallExpression node)
        {
            if (Enter(node))
            {
                node.Member.Accept(this);
                if (node.Arguments != null)
                    node.Arguments.Accept(this);
                foreach (var suffix in node.Suffixes)
                    suffix.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(ConditionalExpression node)
        {
            if (Enter(node))
            {
                node.Condition.Accept(this);
                if (node.TrueExpression != null)
                {
                    node.TrueExpression.Accept(this);
                    node.FalseExpression.Accept(this);
                }
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(EqualityExpression node)
        {
            if (Enter(node))
            {
                node.LeftExpression.Accept(this);
                if (node.RightExpression != null)
                    node.RightExpression.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(ExecExpression node)
        {
            if (Enter(node))
            {
                node.CodeString.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(Expression node)
        {
            if (Enter(node))
            {
                foreach (var exp in node.AssignmentExpressions)
                {
                    exp.Accept(this);
                }
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(FunctionExpression node)
        {
            if (Enter(node))
            {
                if (node.Identifier != null)
                    node.Identifier.Accept(this);
                foreach (var param in node.FormalParameters)
                {
                    param.Accept(this);
                }
                node.Body.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(Identifier node)
        {
            if (Enter(node))
            {
                // Insert code here
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(IndexSuffix node)
        {
            if (Enter(node))
            {
                node.Expression.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(LeftAssignmentExpression node)
        {
            if (Enter(node))
            {
                node.Variable.Accept(this);
                node.Operand.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(MemberExpression node)
        {
            if (Enter(node))
            {
                node.Expression.Accept(this);
                foreach (var suffix in node.Suffixes)
                {
                    suffix.Accept(this);
                }
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(MultiplicativeExpression node)
        {
            if (Enter(node))
            {
                node.LeftExpression.Accept(this);
                if (node.RightExpression != null)
                    node.RightExpression.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(TypeDescriptorSuffix node)
        {
            if (Enter(node))
            {
                foreach (var typeDescriptor in node.TypeDescriptors)
                {
                    typeDescriptor.Accept(this);
                }
            }
            Exit(node);
            return null;
        }


        public virtual object Walk(TypeDescriptor node)
        {
            if (Enter(node))
            {
                foreach (var typeDescriptor in node.TypeDescriptors)
                {
                    typeDescriptor.Accept(this);
                }
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(OrExpression node)
        {
            if (Enter(node))
            {
                node.LeftExpression.Accept(this);
                if (node.RightExpression != null)
                    node.RightExpression.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(PropertyReferenceSuffix node)
        {
            if (Enter(node))
            {
                node.Identifier.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(RelationalExpression node)
        {
            if (Enter(node))
            {
                node.LeftExpression.Accept(this);
                if (node.RightExpression != null)
                    node.RightExpression.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(SelfExpression node)
        {
            if (Enter(node))
            {
                // Insert code here
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(UnaryExpression node)
        {
            if (Enter(node))
            {
                node.Expression.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(ArrayLiteral node)
        {
            if (Enter(node))
            {
                foreach (var exp in node.Expressions)
                {
                    exp.Accept(this);
                }
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(ObjectLiteral node)
        {
            if (Enter(node))
            {
                foreach (var prop in node.Properties)
                {
                    prop.Key.Accept(this);
                    prop.Value.Accept(this);
                }
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(PrimitiveLiteral node)
        {
            if (Enter(node))
            {
                // Insert code here
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(BreakStatement node)
        {
            if (Enter(node))
            {
                // Insert code here
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(CaseClause node)
        {
            if (Enter(node))
            {
                if (node.Expression != null)
                    node.Expression.Accept(this);
                foreach (var statement in node.Body)
                {
                    statement.Accept(this);
                }
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(EmptyStatement node)
        {
            if (Enter(node))
            {
                // Insert code here
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(ExpressionStatement node)
        {
            if (Enter(node))
            {
                node.Expression.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(ForInStatement node)
        {
            if (Enter(node))
            {
                if (node.VariableDeclaration != null)
                    node.VariableDeclaration.Accept(this);
                else
                    node.LeftHandSideExpression.Accept(this);
                node.Collection.Accept(this);
                node.Body.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(IfStatement node)
        {
            if (Enter(node))
            {
                node.Condition.Accept(this);
                node.Body.Accept(this);
                if (node.Else != null)
                    node.Else.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(LoadStatement node)
        {
            if (Enter(node))
            {
                node.CodeFile.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(NextStatement node)
        {
            if (Enter(node))
            {
                // Insert code here
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(RescueClause node)
        {
            if (Enter(node))
            {
                if (node.Identifier != null)
                    node.Identifier.Accept(this);
                node.StatementBlock.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(ReturnStatement node)
        {
            if (Enter(node))
            {
                if (node.Expression != null)
                    node.Expression.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(StatementBlock node)
        {
            if (Enter(node))
            {
                foreach (var statement in node.Statements)
                {
                    statement.Accept(this);
                }
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(SwitchStatement node)
        {
            if (Enter(node))
            {
                node.Condition.Accept(this);
                foreach (var caseClause in node.Cases)
                {
                    caseClause.Accept(this);
                }
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(ThrowStatement node)
        {
            if (Enter(node))
            {
                node.Expression.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(TryStatement node)
        {
            if (Enter(node))
            {
                node.Body.Accept(this);
                if (node.Rescue != null)
                    node.Rescue.Accept(this);
                if (node.Finally != null)
                    node.Finally.Accept(this);
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(VariableStatement node)
        {
            if (Enter(node))
            {
                foreach (var variableDeclaration in node.VariableDeclarations)
                {
                    variableDeclaration.Accept(this);
                }
            }
            Exit(node);
            return null;
        }

        public virtual object Walk(WhileStatement node)
        {
            if (Enter(node))
            {
                node.Condition.Accept(this);
                node.Body.Accept(this);
            }
            Exit(node);
            return null;
        }
    }
}
