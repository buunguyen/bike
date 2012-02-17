namespace Bike.Parser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Ast;

    class Parser : BacktrackParser
    {
        public Parser(Lexer lexer) : base(lexer)
        {
        }

        public SourceUnitTree Parse()
        {
            return ParseProgram();
        }

        public SourceUnitTree ParseProgram()
        {
            if (Next().Is(TokenType.Eof))
                return new SourceUnitTree { Token = Next() };
            var tree = new SourceUnitTree { Token = Next(), SourceElements = ParseSourceElements() };
            Match(TokenType.Eof);
            return tree;
        }

        public SourceElements ParseSourceElements()
        {
            var sourceElements = new SourceElements { Token = Next() };
            while (Next().IsNot(TokenType.Eof) && 
                   Next().IsNot(TokenType.RightBrace)) 
            {
                var statement = Next().Is(TokenType.Func) 
                    ? ParseFunctionDeclaration() 
                    : ParseStatement();
                sourceElements.Statements.Add(statement);
            }
            return sourceElements;
        }

        public FunctionDeclaration ParseFunctionDeclaration()
        {
            var funcDec = new FunctionDeclaration { Token = Next() };
            Match(TokenType.Func);
            funcDec.Identifier = ParseIdentifier();
            funcDec.FormalParameters = ParseFormalParameterList();
            funcDec.Body = ParseFunctionBody();
            return funcDec;
        }

        public FunctionExpression ParseFunctionExpression()
        {
            var funcDec = new FunctionExpression { Token = Next() };
            Match(TokenType.Func);
            if (Next().Is(TokenType.Identifier))
            {
                funcDec.Identifier = ParseIdentifier();
            }
            funcDec.FormalParameters = ParseFormalParameterList();
            funcDec.Body = ParseFunctionBody();
            return funcDec;
        }

        private SourceElements ParseFunctionBody()
        {
            Match(TokenType.LeftBrace);
            var body = ParseSourceElements();
            Match(TokenType.RightBrace);
            return body;
        }

        private List<FormalParameter> ParseFormalParameterList()
        {
            var list = new List<FormalParameter>();
            Match(TokenType.LeftParen);
            while (Next().Is(TokenType.Multiply) ||
                   Next().Is(TokenType.Identifier))
            {
                var parameter = new FormalParameter() {Token = Next()};
                if (Next().Is(TokenType.Multiply))
                {
                    Match(TokenType.Multiply);
                    parameter.IsParams = true;
                }
                parameter.Identifier = ParseIdentifier();
                if (list.Any(param => param.Identifier.Equals(parameter.Identifier)))
                    throw Error("Duplicated parameter name", Next());

                list.Add(parameter);
                if (Next().Is(TokenType.Comma))
                {
                    Match(TokenType.Comma);
                    continue;
                }
                if (Next().Is(TokenType.RightParen))
                    break;
                throw Error("Unexpected token", Next());
            }

            if (list.Any(p => p.IsParams && list.IndexOf(p) != list.Count - 1))
                throw Error("Var-arg parameter must be the last parameter", Next());

            Match(TokenType.RightParen);
            return list;
        }

        private Identifier ParseIdentifier()
        {
            var token = Next();
            Match(TokenType.Identifier);
            return new Identifier { Value = token.Text, Token = token };
        }

        public Statement ParseStatement()
        {
            switch (Next().Type)
            {
                case TokenType.LeftBrace:
                    {
                        Statement node;
                        return Speculate(ParseExpressionStatement, out node)
                            ? node
                            : ParseStatementBlock();
                    }
                case TokenType.Load:
                    return ParseLoadStatement();
                case TokenType.Var:
                    return ParseVariableStatement();
                case TokenType.SemiColon:
                    return ParseEmptyStatement();
                case TokenType.If:
                    return ParseIfStatement();
                case TokenType.While:
                    return ParseWhileStatement();
                case TokenType.For:
                    return ParseForInStatement();
                case TokenType.Next:
                    return ParseNextStatement();
                case TokenType.Break:
                    return ParseBreakStatement();
                case TokenType.Return:
                    return ParseReturnStatement();
                case TokenType.Switch:
                    return ParseSwitchStatement();
                case TokenType.Throw:
                    return ParseThrowStatement();
                case TokenType.Try:
                    return ParseTryStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        public StatementBlock ParseStatementBlock()
        {
            var block = new StatementBlock { Token = Next() };
            Match(TokenType.LeftBrace);
            while (Next().IsNot(TokenType.RightBrace))
                block.Statements.Add(ParseStatement());
            Match(TokenType.RightBrace);
            return block;
        }

        private LoadStatement ParseLoadStatement()
        {
            var import = new LoadStatement { Token = Next() };
            Match(TokenType.Load);
            import.CodeFile = ParseAssignmentExpression();
            Match(TokenType.SemiColon);
            return import;
        }

        public VariableStatement ParseVariableStatement()
        {
            var varStatement = new VariableStatement { Token = Next() };
            Match(TokenType.Var);

            varStatement.VariableDeclarations.Add(ParseVariableDeclaration());
            while (Next().Is(TokenType.Comma))
            {
                Match(TokenType.Comma);
                varStatement.VariableDeclarations.Add(ParseVariableDeclaration());
            }

            Match(TokenType.SemiColon);
            return varStatement;
        }

        public VariableDeclaration ParseVariableDeclaration()
        {
            var varDec = new VariableDeclaration
                             {
                                 Token = Next(),
                                 Identifier = ParseIdentifier()
                             };
            if (Next().Is(TokenType.OpAssign))
            {
                Match(TokenType.OpAssign, "=");
                varDec.Initializer = ParseAssignmentExpression();
            }
            return varDec;
        }

        public EmptyStatement ParseEmptyStatement()
        {
            Match(TokenType.SemiColon);
            return new EmptyStatement {Token = Next()};
        }

        public ExpressionStatement ParseExpressionStatement()
        {
            var expStatement = new ExpressionStatement
                       {
                           Token = Next(),
                           Expression = ParseExpression()
                       };

            // TODO: 
            // - Any formal way to handle this?
            // - How to handle new line as well?

            if (Next().Is(TokenType.RightBrace))
                return expStatement;
            Match(TokenType.SemiColon);
            return expStatement;
        }

        public IfStatement ParseIfStatement()
        {
            var ifStatement = new IfStatement { Token = Next() };
            Match(TokenType.If);
            Match(TokenType.LeftParen);
            ifStatement.Condition = ParseAssignmentExpression();
            Match(TokenType.RightParen);
            ifStatement.Body = ParseStatement();
            if (Next().Is(TokenType.Else))
            {
                Match(TokenType.Else);
                ifStatement.Else = ParseStatement();
            }
            return ifStatement;
        }

        public WhileStatement ParseWhileStatement()
        {
            var whileStatement = new WhileStatement { Token = Next() };
            Match(TokenType.While);
            Match(TokenType.LeftParen);
            whileStatement.Condition = ParseAssignmentExpression();
            Match(TokenType.RightParen);
            whileStatement.Body = ParseStatement();
            return whileStatement;
        }

        public ForInStatement ParseForInStatement()
        {
            var forInStatement = new ForInStatement { Token = Next() };
            Match(TokenType.For);
            Match(TokenType.LeftParen);
            if (Next().Is(TokenType.Var))
            {
                Match(TokenType.Var);
                forInStatement.VariableDeclaration = ParseVariableDeclaration();
            }
            else
            {
                forInStatement.LeftHandSideExpression = ParseLeftHandSideExpression();
            }
            Match(TokenType.In);
            forInStatement.Collection = ParseAssignmentExpression();
            Match(TokenType.RightParen);
            forInStatement.Body = ParseStatement();
            return forInStatement;
        }

        public NextStatement ParseNextStatement()
        {
            var nextStatement = new NextStatement { Token = Next() };
            Match(TokenType.Next); 
            Match(TokenType.SemiColon);
            return nextStatement;
        }

        public BreakStatement ParseBreakStatement()
        {
            var breakStatement = new BreakStatement { Token = Next() };
            Match(TokenType.Break);
            Match(TokenType.SemiColon);
            return breakStatement;
        }

        public ReturnStatement ParseReturnStatement()
        {
            var returnStatement = new ReturnStatement { Token = Next() };
            Match(TokenType.Return);
            if (Next().Type == TokenType.SemiColon)
            {
                Match(TokenType.SemiColon);
                return returnStatement;
            }
            returnStatement.Expression = ParseAssignmentExpression(); 
            Match(TokenType.SemiColon);
            return returnStatement;
        }

        public SwitchStatement ParseSwitchStatement()
        {
            var switchStatement = new SwitchStatement { Token = Next() };
            Match(TokenType.Switch);
            Match(TokenType.LeftParen);
            switchStatement.Condition = ParseAssignmentExpression();
            Match(TokenType.RightParen);
            Match(TokenType.LeftBrace);
            while (Next().IsNot(TokenType.RightBrace))
                switchStatement.Cases.Add(ParseCaseClause());
            Match(TokenType.RightBrace);
            return switchStatement;
        }

        public CaseClause ParseCaseClause()
        {
            var caseClause = new CaseClause { Token = Next() };
            if (Next().Is(TokenType.Case))
            {
                Match(TokenType.Case);
                caseClause.Expression = ParseExpression();
                Match(TokenType.Colon);
            }
            else if (Next().Is(TokenType.Default))
            {
                Match(TokenType.Default);
                Match(TokenType.Colon);
            }
            caseClause.Body = ParseCaseStatementList();
            return caseClause;
        }

        private List<Statement> ParseCaseStatementList()
        {
            var statements = new List<Statement>();
            while (Next().IsNot(TokenType.RightBrace) &&
                   Next().IsNot(TokenType.Case) &&
                   Next().IsNot(TokenType.Default))
            {
                statements.Add(ParseStatement());
            }
            return statements;
        }

        public ThrowStatement ParseThrowStatement()
        { 
            var throwStatement = new ThrowStatement { Token = Next() };
            Match(TokenType.Throw);
            throwStatement.Expression = ParseAssignmentExpression();
            Match(TokenType.SemiColon);
            return throwStatement;
        }

        public TryStatement ParseTryStatement()
        {
            var tryStatement = new TryStatement { Token = Next() };
            Match(TokenType.Try);
            tryStatement.Body = ParseStatementBlock();
            if (Next().IsNot(TokenType.Rescue) && Next().IsNot(TokenType.Finally))
                throw Error("Missing rescue or finally", Next());

            if (Next().Is(TokenType.Rescue))
            {
                var rescueClause = new RescueClause { Token = Next() };
                Match(TokenType.Rescue);
                if (!Next().Is(TokenType.LeftBrace))
                {
                    rescueClause.Identifier = ParseIdentifier();   
                }
                rescueClause.StatementBlock = ParseStatementBlock();
                tryStatement.Rescue = rescueClause;
            }
            if (Next().Is(TokenType.Finally))
            {
                Match(TokenType.Finally);
                tryStatement.Finally = ParseStatementBlock();
            }
            return tryStatement;
        }

        public ExprNode ParseExpression()
        {
            var exp = new Expression {Token = Next()};
            var assignmentExp = ParseAssignmentExpression();
            exp.AssignmentExpressions.Add(assignmentExp);
            if (Next().IsNot(TokenType.Comma))
                return assignmentExp;

            while (Next().Is(TokenType.Comma))
            {
                Match(TokenType.Comma);
                exp.AssignmentExpressions.Add(ParseAssignmentExpression());
            }
            return exp;
        }

        public ExprNode ParseAssignmentExpression()
        {
            if (Next().Is(TokenType.Exec))
            {
                var evalExp = new ExecExpression { Token = Next() };
                Match(TokenType.Exec);
                evalExp.CodeString = ParseAssignmentExpression();
                return evalExp;
            }
            ExprNode node;
            return Speculate(ParseLeftAssignmentExpression, out node) 
                ? node 
                : ParseConditionalExpression();
        }

        private LeftAssignmentExpression ParseLeftAssignmentExpression()
        {
            var exp = new LeftAssignmentExpression
                          {
                              Token = Next(),
                              Variable = ParseLeftHandSideExpression(),
                              Operator = Next()
                          };
            Match(TokenType.OpAssign);
            exp.Operand = ParseAssignmentExpression();
            return exp;
        }

        public ExprNode ParseLeftHandSideExpression()
        {
            var callExp = new CallExpression { Token = Next() };
            callExp.Member = ParseMemberExpression();
            if (Next().IsNot(TokenType.LeftParen))
                return callExp.Member;
            callExp.Arguments = ParseArguments();
            while (true)
            {
                switch (Next().Type)
                {
                    case TokenType.LeftParen:
                        callExp.Suffixes.Add(ParseArguments());
                        break;
                    case TokenType.LeftBracket:
                        callExp.Suffixes.Add(ParseIndexSuffix());
                        break;
                    case TokenType.Dot:
                        callExp.Suffixes.Add(ParsePropertyReferenceSuffix());
                        break;
                    case TokenType.LessThan:
                        Node node;
                        if (Speculate(ParseTypeDescriptorSuffix, out node))
                        {
                            callExp.Suffixes.Add(node);
                        }
                        else
                        {
                            return callExp;
                        }
                        break;
                    default:
                        return callExp;
                }
            }
        }

        private TypeDescriptorSuffix ParseTypeDescriptorSuffix()
        {
            var tds = new TypeDescriptorSuffix() { Token = Next() };
            Match(TokenType.LessThan);
            tds.TypeDescriptors.Add(ParseTypeDescriptor());
            while (Next().Is(TokenType.Comma))
            {
                Match(TokenType.Comma);
                tds.TypeDescriptors.Add(ParseTypeDescriptor());
            }
            Match(TokenType.GreaterThan);
            return tds;
        }

        private TypeDescriptor ParseTypeDescriptor()
        {
            var td = new TypeDescriptor() { Token = Next() };
            var name = new StringBuilder();
            name.Append(ParseIdentifier().Value);
            bool hasTypeDescriptor = false;
            while (Next().Is(TokenType.Dot) || Next().Is(TokenType.LessThan))
            {
                if (Next().Is(TokenType.Dot))
                {
                    if (hasTypeDescriptor)
                        Error("Invalid type definition", Next());
                    Match(TokenType.Dot);
                    name.Append(".");
                    name.Append(ParseIdentifier().Value);
                }
                else
                {
                    hasTypeDescriptor = true;
                    Match(TokenType.LessThan);
                    td.TypeDescriptors.Add(ParseTypeDescriptor());
                    while (Next().Is(TokenType.Comma))
                    {
                        Match(TokenType.Comma);
                        td.TypeDescriptors.Add(ParseTypeDescriptor());
                    }
                    Match(TokenType.GreaterThan);
                }
            }

            td.Name = name.ToString();
            return td;
        }

        private ExprNode ParseMemberExpression()
        {
            var exp = new MemberExpression {Token = Next()};
            exp.Expression = Next().Is(TokenType.Func) 
                ? ParseFunctionExpression() 
                : ParsePrimaryExpression();
            exp.Suffixes = ParseMemberExpressionSuffix();
            return exp.Suffixes.Count == 0 
                ? exp.Expression 
                : exp;
        }

        private List<Node> ParseMemberExpressionSuffix()
        {
            var suffixes = new List<Node>();
            var cont = true;
            while (cont)
            {
                switch (Next().Type)
                {
                    case TokenType.LeftBracket:
                        suffixes.Add(ParseIndexSuffix());
                        break;
                    case TokenType.Dot:
                        suffixes.Add(ParsePropertyReferenceSuffix());
                        break;
                    case TokenType.LessThan:
                        Node node;
                        if (Speculate(ParseTypeDescriptorSuffix, out node))
                        {
                            suffixes.Add(node);
                        }
                        else
                        {
                            return suffixes;
                        }
                        break;
                    default:
                        cont = false;
                        break;
                }
            }
            return suffixes;
        }

        public Arguments ParseArguments()
        {
            var args = new Arguments {Token = Next()};
            Match(TokenType.LeftParen);
            if (Next().Is(TokenType.RightParen))
            {
                Match(TokenType.RightParen);
                return args;
            }
            args.Children.Add(ParseArgument());
            while (Next().Is(TokenType.Comma))
            {
                Match(TokenType.Comma);
                args.Children.Add(ParseArgument());
            }
            Match(TokenType.RightParen);
            return args;
        }

        private Argument ParseArgument()
        {
            var arg = new Argument { Token = Next() };
            if (Next().Is(TokenType.Multiply)) // i.e. '*', bad name, I know
            {
                Match(TokenType.Multiply);
                arg.ShouldExpand = true;
            }
            arg.Expression = ParseAssignmentExpression();
            return arg;
        }

        public IndexSuffix ParseIndexSuffix()
        {
            var suffix = new IndexSuffix {Token = Next()};
            Match(TokenType.LeftBracket);
            suffix.Expression = ParseExpression();
            Match(TokenType.RightBracket);
            return suffix;
        }

        public PropertyReferenceSuffix ParsePropertyReferenceSuffix()
        {
            var suffix = new PropertyReferenceSuffix { Token = Next() };
            Match(TokenType.Dot);
            suffix.Identifier = ParseIdentifier();
            return suffix;
        }

        public ExprNode ParseConditionalExpression()
        {
            var exp = new ConditionalExpression
                          {
                              Token = Next(), 
                              Condition = ParseLogicalOrExpression()
                          };
            if (Next().IsNot(TokenType.Question))
                return exp.Condition;

            Match(TokenType.Question);
            exp.TrueExpression = ParseAssignmentExpression();
            Match(TokenType.Colon);
            exp.FalseExpression = ParseAssignmentExpression();
            return exp;
        }

        public ExprNode ParseLogicalOrExpression()
        {
            var exp = new OrExpression
                          {
                              Token = Next(), 
                              LeftExpression = ParseLogicalAndExpression()
                          };
            if (Next().IsNot(TokenType.LogicalOr))
                return exp.LeftExpression;
            Match(TokenType.LogicalOr);
            exp.RightExpression = ParseLogicalOrExpression();
            return exp;
        }

        public ExprNode ParseLogicalAndExpression()
        {
                var exp = new AndExpression
                          {
                              Token = Next(), 
                              LeftExpression = ParseEqualityExpression()
                          };
            if (Next().IsNot(TokenType.LogicalAnd))
                return exp.LeftExpression;
            
            Match(TokenType.LogicalAnd);
            exp.RightExpression = ParseLogicalAndExpression();
            return exp;
        }

        public ExprNode ParseEqualityExpression()
        {
            var exp = new EqualityExpression
                          {
                              Token = Next(),
                              LeftExpression = ParseRelationalExpression()
                          };
            if (Next().IsNot(TokenType.Equal) && Next().IsNot(TokenType.NotEqual))
                return exp.LeftExpression;
            exp.Operator = Next().Type;
            Consume();
            exp.RightExpression = ParseEqualityExpression();
            return exp;
        }

        public ExprNode ParseRelationalExpression()
        {
            var exp = new RelationalExpression
                          {
                              Token = Next(),
                              LeftExpression = ParseAdditiveExpression(),
                          };
            if (Next().IsNot(TokenType.LessThan) &&
                Next().IsNot(TokenType.LessThanOrEqual) &&
                Next().IsNot(TokenType.GreaterThan) &&
                Next().IsNot(TokenType.GreaterThanOrEqual) &&
                Next().IsNot(TokenType.Is))
                return exp.LeftExpression;
            
            exp.Operator = Next().Type;
            Consume();
            exp.RightExpression = ParseRelationalExpression();
            return exp;
        }

        public ExprNode ParseAdditiveExpression()
        {
            var exp = new AdditiveExpression
                          {
                              Token = Next(),
                              LeftExpression = ParseMultiplicativeExpression()
                          };
            if (Next().IsNot(TokenType.Plus) &&
                Next().IsNot(TokenType.Minus))
                return exp.LeftExpression;
            
            exp.Operator = Next().Type;
            Consume();
            exp.RightExpression = ParseAdditiveExpression();
            return exp;
        }

        public ExprNode ParseMultiplicativeExpression()
        {
            var exp = new MultiplicativeExpression
                          {
                              Token = Next(),
                              LeftExpression = ParseUnaryExpression()
                          };
            if (Next().IsNot(TokenType.Multiply) &&
                Next().IsNot(TokenType.Divide) &&
                Next().IsNot(TokenType.Modulus))
                return exp.LeftExpression;
            
            exp.Operator = Next().Type;
            Consume();
            exp.RightExpression = ParseMultiplicativeExpression();
            return exp;
        }

        public ExprNode ParseUnaryExpression()
        {
            var exp = new UnaryExpression
                          {
                              Token = Next(),
                              Postfix = TokenType.None,
                              Prefix = TokenType.None
                          };
            var token = Next();
            if (token.Is(TokenType.DoublePlus) || 
                token.Is(TokenType.DoubleMinus) || 
                token.Is(TokenType.Plus) || 
                token.Is(TokenType.Minus) || 
                token.Is(TokenType.Not))
            {
                exp.Prefix = token.Type;
                Consume();
                exp.Expression = ParseUnaryExpression();
                return exp;
            }

            exp.Expression = ParseLeftHandSideExpression();
            if (Next().Is(TokenType.DoublePlus) || Next().Is(TokenType.DoubleMinus))
            {
                exp.Postfix = Next().Type;
                Consume();
                return exp;
            }

            return exp.Expression;
        }

        public ExprNode ParsePrimaryExpression()
        {
            switch (Next().Type)
            {
                case TokenType.This:
                    return ParseSelfExpression();
                case TokenType.Identifier:
                    return ParseIdentifier();
                case TokenType.LeftBracket:
                    return ParseArrayLiteral();
                case TokenType.LeftBrace:
                    return ParseObjectLiteral();
                case TokenType.LeftParen:
                    Match(TokenType.LeftParen);
                    var exp = ParseExpression();
                    Match(TokenType.RightParen);
                    return exp;
                default:
                    return ParsePrimitiveLiteral();
            }
        }

        private SelfExpression ParseSelfExpression()
        {
            var exp = new SelfExpression {Token = Next()};
            Match(TokenType.This);
            return exp;
        }

        public ArrayLiteral ParseArrayLiteral()
        {
            var al = new ArrayLiteral {Token = Next()};
            Match(TokenType.LeftBracket);
            while (Next().IsNot(TokenType.RightBracket))
            {
                al.Expressions.Add(ParseAssignmentExpression());
                if (Next().Is(TokenType.Derive))
                {
                    if (al.Expressions.Count != 1)
                        throw Error("Invalid array definition", al.Token);
                    Match(TokenType.Derive);
                    al.Expressions.Add(ParseAssignmentExpression());
                    al.IsRange = true;
                    break;
                }
                if (Next().Is(TokenType.Comma)) 
                    Match(TokenType.Comma);
            }
            Match(TokenType.RightBracket);
            return al;
        }

        public ObjectLiteral ParseObjectLiteral()
        {
            var ol = new ObjectLiteral {Token = Next()};
            Match(TokenType.LeftBrace);
            while (Next().IsNot(TokenType.RightBrace))
            {
                var key = ParsePropertyName();
                Match(TokenType.Colon);
                var value = ParseAssignmentExpression();
                ol.Properties.Add(key, value);
                if (Next().Is(TokenType.Comma))
                    Match(TokenType.Comma);
                else break;
            }
            Match(TokenType.RightBrace);
            return ol;
        }

        public Node ParsePropertyName()
        {
            var token = Next();
            if (token.Is(TokenType.Identifier))
                return ParseIdentifier();
            if (token.Is(TokenType.String) || token.Is(TokenType.Number))
                return ParsePrimitiveLiteral();
            throw Error("Invalid property name", token);
        }

        public PrimitiveLiteral ParsePrimitiveLiteral()
        {
            var token = Next();
            if (!token.Is(TokenType.Null) &&
                !token.Is(TokenType.True) &&
                !token.Is(TokenType.False) &&
                !token.Is(TokenType.String) &&
                !token.Is(TokenType.Number))
                throw Error("Primitive literal expected, received " + token.Type, token);
            var literal = new PrimitiveLiteral
                              {
                                  Token = Next(), 
                                  Value = Next().Text, 
                                  Type = Next().Type
                              };
            Consume();
            return literal;
        }
    }
}
