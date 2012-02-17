namespace Bike.Interpreter
{
    using System;
    using System.Collections;
    using System.IO;
    using Ast;
    using Builtin;
    using Parser;

    public partial class Interpreter : NodeWalker
    {
        internal readonly InterpretationContext Context;

        internal Interpreter(InterpretationContext context)
        {
            Context = context;
        }

        public void Run(string coreCodePath, string filePath = null)
        {
            try
            {
                LoadAndExecute(coreCodePath);
            }
            catch(Exception e)
            {
                if (!(e is InterpreterException))
                    throw new InterpreterException("Cannot load core library", e);
                throw;
            }
            Func<object> func = () =>
                             {
                                 if (filePath != null)
                                     LoadAndExecute(filePath);
                                 return null;
                             };
            InterpreterHelper.ActAndHandleException(func);
        }

        internal object LoadAndExecute(string filePath)
        {
            bool alreadyImport;
            var fullPath = Context.ImportFile(filePath, out alreadyImport);
            if (!alreadyImport)
            {
                return Execute(File.ReadAllText(fullPath), fullPath);
            }
            return null;
        }

        internal object Execute(string source, string filePath = null)
        {
            var oldCurentExecFolder = Context.CurrentExecFolder;
            var currentExecFolder = filePath == null
                ? Directory.GetCurrentDirectory()
                : Path.GetDirectoryName(filePath);
            Context.CurrentExecFolder = currentExecFolder;
            if (currentExecFolder != null)
                Directory.SetCurrentDirectory(currentExecFolder);

            try
            {
                var lexer = new Lexer(source, filePath);
                var parser = new Parser(lexer);
                var ast = parser.Parse();
                return ast.Accept(this);
            }
            finally
            {
                Context.CurrentExecFolder = oldCurentExecFolder;
                if (oldCurentExecFolder != null)
                    Directory.SetCurrentDirectory(oldCurentExecFolder);
            }
        }

        #region Implementation of IWalker
        public override object Walk(VariableDeclaration node)
        {
            var value = node.Initializer == null
                ? null
                : node.Initializer.Accept(this);
            Context.CurrentFrame.Define(node.Identifier.Value, value);
            return value;
        }

        public override object Walk(EqualityExpression node)
        {
            var lv = node.LeftExpression.Accept(this);
            var rv = node.RightExpression.Accept(this);
            switch (node.Operator)
            {
                case TokenType.Equal:
                    return OpEqual(lv, rv);
                case TokenType.NotEqual:
                    return OpNotEqual(lv, rv);
                default:
                    throw ErrorFactory.CreateInvalidProgramError("Invalid equality operator");
            }
        }

        public override object Walk(WhileStatement node)
        {
            object result = null;
            while (InterpreterHelper.IsTrue(node.Condition.Accept(this)))
            {
                try
                {
                    result = node.Body.Accept(this);
                } 
                catch (Break)
                {
                    break;
                }
                catch (Next)
                {
                    continue;
                }
            }
            return result;
        }

        public override object Walk(BreakStatement node)
        {
            throw new Break();
        }

        public override object Walk(NextStatement node)
        {
            
            throw new Next();
        }

        public override object Walk(SwitchStatement node)
        {
            var v = node.Condition.Accept(this);
            var hasFirstMatch = false;
            object result = null;
            foreach (var caseClause in node.Cases)
            {
                if (!hasFirstMatch && 
                    !caseClause.IsDefault && 
                    !CaseMatch(caseClause, v))
                    continue;
                hasFirstMatch = true;
                try
                {
                    result = caseClause.Accept(this);
                }
                catch (Break)
                {
                    break;
                }
            }
            return result;
        }

        private bool CaseMatch(CaseClause caseClause, object o)
        {
            var conditions = caseClause.Expression.Accept(this);
            if (caseClause.Expression is Expression)
            {
                foreach (var condition in (object[])conditions)
                {
                    if (OpEqual(condition, o).Value) 
                        return true;
                }
                return false;
            }
            return OpEqual(conditions, o).Value;
        }

        public override object Walk(CaseClause node)
        {
            object result = null;
            foreach (var statement in node.Body)
            {
                result = statement.Accept(this);
            }
            return result;
        }

        public override object Walk(ForInStatement node)
        {
            var collection = node.Collection.Accept(this);
            if (collection == null)
                throw ErrorFactory.CreateNullError("Collection");
            if (!typeof(IEnumerable).IsAssignableFrom(collection.GetType()))
                throw ErrorFactory.CreateTypeError(string.Format("Object {0} is not enumerable", collection));

            string id;
            bool hasDeclaration = false;
            if (node.VariableDeclaration == null)
            {
                id = ((Identifier)node.LeftHandSideExpression.Accept(this)).Value;
            }
            else
            {
                id = node.VariableDeclaration.Identifier.Value;
                hasDeclaration = true;
            }

            object result = null;
            Action body = delegate
                              {
                                  foreach (var element in (IEnumerable)collection)
                                  {
                                      Context.CurrentFrame.Assign(id, element);
                                      try { result = node.Body.Accept(this); }
                                      catch (Break) { break; }
                                      catch (Next) { continue; }
                                  }
                              };
            Action<ScopeFrame> init = scopeFrame => scopeFrame.Define(id, null);
            Context.OpenScopeFor(body, withInit: init, when: hasDeclaration);
            return result;
        }

        public override object Walk(FunctionDeclaration node)
        {
            var funcDec = new BikeFunction(node, node.Identifier, node.FormalParameters, node.Body, 
                Context.CurrentFrame);
            Context.CurrentFrame.Define(node.Identifier.Value, funcDec);
            return funcDec;
        }

        public override object Walk(FunctionExpression node)
        {
            var funcDec = new BikeFunction(node, node.Identifier, node.FormalParameters, node.Body,
                Context.CurrentFrame);
            if (node.Identifier != null)
                Context.CurrentFrame.Define(node.Identifier.Value, funcDec);
            return funcDec;
        }

        public override object Walk(SelfExpression node)
        {
            return Context.CurrentFrame.Resolve("this");
        }

        public override object Walk(EmptyStatement node)
        {
            return null;
        }

        public override object Walk(ReturnStatement node)
        {
            var retVal = node.Expression == null 
                ? null 
                : node.Expression.Accept(this);
            throw new Return(retVal);
        }

        public override object Walk(ThrowStatement node)
        {
            var stackTrace = ErrorFactory.BuildStackTrace();
            var throwable = node.Expression.Accept(this);
            if (throwable == null)
                throw ErrorFactory.CreateNullError("Throwable");
            if (throwable is BikeObject)
            {
                var bo = (BikeObject)throwable;
                if (!bo.OwnExist("stack_trace"))
                    bo.Members["stack_trace"] = stackTrace;
                throw bo;
            }

            var error = (throwable is Exception)
                        ? ErrorFactory.CreateClrError((Exception) throwable)
                        : ErrorFactory.CreateTypeError("Can only throw Bike.Object or CLR's Exception");
            error.Members["stack_trace"] = stackTrace;
            throw error;
        }

        public override object Walk(TryStatement node)
        {
            object result = null;
            try
            {
                result = node.Body.Accept(this);
            } 
            catch (Exception e)
            {
                if (e is ControlFlow || node.Rescue == null)
                    throw;
                Action body = () => node.Rescue.Accept(this);
                Action<ScopeFrame> init = scopeFrame => scopeFrame.Define(node.Rescue.Identifier.Value, e);
                Context.OpenScopeFor(body, withInit: init, when: node.Rescue.Identifier != null);
            }
            finally
            {
                if (node.Finally != null)
                    node.Finally.Accept(this);
            }
            return result;
        }

        public override object Walk(IfStatement node)
        {
            var cond = node.Condition.Accept(this);
            if (InterpreterHelper.IsTrue(cond))
            {
                return node.Body.Accept(this);
            }
            return node.Else != null ? node.Else.Accept(this) : null;
        }

        public override object Walk(ConditionalExpression node)
        {
            var cond = node.Condition.Accept(this);
            return InterpreterHelper.IsTrue(cond) 
                ? node.TrueExpression.Accept(this) 
                : node.FalseExpression.Accept(this);
        }

        public override object Walk(SourceUnitTree node)
        {
            return node.SourceElements == null
                ? null
                : node.SourceElements.Accept(this);
        }

        public override object Walk(SourceElements node)
        {
            object result = null;
            foreach (var statement in node.Statements)
            {
                result = statement.Accept(this);
            }
            return result;
        }

        public override object Walk(StatementBlock node)
        {
            object result = null;
            Action action = delegate
            {
                foreach (var statement in node.Statements)
                {
                    result = statement.Accept(this);
                }
            };
            Context.OpenScopeFor(action);
            return result;
        }

        public override object Walk(TypeDescriptorSuffix node)
        {
            var types = new Type[node.TypeDescriptors.Count];
            for (int i = 0; i < types.Length; i++)
            {
                types[i] = (Type)node.TypeDescriptors[i].Accept(this);
            }
            return types;
        }

        public override object Walk(TypeDescriptor node)
        {
            return Context.LoadTypeFromDescriptor(node);
        }

        public override object Walk(ArrayLiteral node)
        {
            var array = new BikeArray();
            if (node.IsRange)
            {
                var from = node.Expressions[0].Accept(this);
                var fromValue = ((BikeNumber) from).Value;
                if ((fromValue % 1) != 0)
                    throw ErrorFactory.CreateTypeError("Array range initializer must start with a whole number");

                var end = node.Expressions[1].Accept(this);
                var endValue = ((BikeNumber)end).Value;
                if ((endValue % 1) != 0)
                    throw ErrorFactory.CreateTypeError("Array range initializer must end with a whole number");

                if (fromValue <= endValue)
                {
                    int index = 0;
                    for (var i = (int) fromValue; i < (int) endValue; i++)
                    {
                        array.Define((index++).ToString(), new BikeNumber(i));
                    }
                }
                else
                {
                    int index = 0;
                    for (var i = (int)fromValue; i > (int)endValue; i--)
                    {
                        array.Define((index++).ToString(), new BikeNumber(i));
                    } 
                }
                return array;
            }
            for (int i = 0; i < node.Expressions.Count; i++)
            {
                var value = node.Expressions[i].Accept(this);
                array.Define(i.ToString(), value); 
            }
            return array;
        }

        public override object Walk(ObjectLiteral node)
        {
            var bikeObject = new BikeObject(InterpretationContext.ObjectBase);
            foreach (var prop in node.Properties)
            {
                string name = prop.Key is Identifier
                                  ? ((Identifier) prop.Key).Value
                                  : ((PrimitiveLiteral) prop.Key).Value;
                var value = prop.Value.Accept(this);
                bikeObject.Assign(name, value);
                if (value is BikeFunction && ((BikeFunction)value).Name.Value == BikeFunction.Anonymous)
                {
                    ((BikeFunction)value).Name = new BikeString(name);
                }
            }
            return bikeObject;
        }

        public override object Walk(IndexSuffix node)
        {
            return node.Expression.Accept(this);
        }

        public override object Walk(OrExpression node)
        {
            var lv = node.LeftExpression.Accept(this);
            Func<object> rvThunk = () => node.RightExpression.Accept(this);
            return OpOr(lv, rvThunk);
        }

        public override object Walk(AndExpression node)
        {
            var lv = node.LeftExpression.Accept(this);
            Func<object> rvThunk = () => node.RightExpression.Accept(this);
            return OpAnd(lv, rvThunk);
        }

        public override object Walk(ExecExpression node)
        {
            var code = node.CodeString.Accept(this);
            if (!(code is BikeString))
                throw ErrorFactory.CreateTypeError("'exec' works only with string type");

            var str = ((BikeString)code).Value;
            return string.IsNullOrWhiteSpace(str) ? null : Execute(str);
        }

        public override object Walk(LoadStatement node)
        {
            var file = node.CodeFile.Accept(this);
            if (!(file is BikeString))
                throw ErrorFactory.CreateTypeError("'load' works only with string type");

            var path = ((BikeString)file).Value;
            if (string.IsNullOrWhiteSpace(path))
                throw ErrorFactory.CreateLoadError(path);

            if (Path.GetExtension(path).ToUpperInvariant() == ".BK")
            {
                return LoadAndExecute(path);
            }
            Context.ImportAssembly(path);
            return null;
        }

        public override object Walk(UnaryExpression node)
        {
            var value = node.Expression.Accept(this);
            if (node.Postfix != TokenType.None)
            {
                switch (node.Postfix)
                {
                    case TokenType.DoublePlus:
                        {
                            var id = node.Expression.As<Identifier>("Identifier").Value;
                            Context.CurrentFrame.Assign(id, OpDoublePlus(value));
                            break;
                        }
                    case TokenType.DoubleMinus:
                        {
                            var id = node.Expression.As<Identifier>("Identifier").Value;
                            Context.CurrentFrame.Assign(id, OpDoubleMinus(value));
                            break;
                        }
                    default:
                        throw ErrorFactory.CreateInvalidProgramError("Invalid unary postfix");
                }
            }
            if (node.Prefix != TokenType.None)
            {
                switch(node.Prefix)
                {
                    case TokenType.DoublePlus:
                        {
                            var id = node.Expression.As<Identifier>("Identifier").Value;
                            value = OpDoublePlus(value);
                            Context.CurrentFrame.Assign(id, value);
                            break;
                        }
                    case TokenType.DoubleMinus:
                        {
                            var id = node.Expression.As<Identifier>("Identifier").Value;
                            value = OpDoubleMinus(value);
                            Context.CurrentFrame.Assign(id, value);
                            break;
                        }
                    case TokenType.Plus:
                        value = OpPlus(value);
                        break;
                    case TokenType.Minus:
                        value = OpMinus(value);
                        break;
                    case TokenType.Not:
                        value = OpNot(value);
                        break;
                    default:
                        throw ErrorFactory.CreateInvalidProgramError("Invalid unary prefix");
                }
            }
            return value;
        }

        public override object Walk(RelationalExpression node)
        {
            var lv = node.LeftExpression.Accept(this);
            switch (node.Operator)
            {
                case TokenType.LessThan:
                    return OpLessThan(lv, node.RightExpression.Accept(this));
                case TokenType.LessThanOrEqual:
                    return OpLessThanOrEqual(lv, node.RightExpression.Accept(this));
                case TokenType.GreaterThan:
                    return OpGreaterThan(lv, node.RightExpression.Accept(this));
                case TokenType.GreaterThanOrEqual:
                    return OpGreaterThanOrEqual(lv, node.RightExpression.Accept(this));
                case TokenType.Is:
                    if (lv == null)
                        return new BikeBoolean(false);
                    var targetValue = node.RightExpression.Accept(this);
                    if (targetValue is BikeObject)
                    {
                        if (!(lv is BikeObject))
                            return new BikeBoolean(false);
                        return ((BikeObject) targetValue).IsPrototypeOf((BikeObject) lv);
                    }
                    if (targetValue is Type)
                        return new BikeBoolean(Is(lv, (Type)targetValue));
                    throw ErrorFactory.CreateTypeError("The right expression must be a Bike.Object or System.Type");
                default:
                    throw ErrorFactory.CreateInvalidProgramError("Invalid relational operator");
            }
        }

        public override object Walk(MultiplicativeExpression node)
        {
            var lv = node.LeftExpression.Accept(this);
            var rv = node.RightExpression.Accept(this);
            switch (node.Operator)
            {
                case TokenType.Multiply:
                    return OpMultiply(lv, rv);
                case TokenType.Divide:
                    return OpDivide(lv, rv);
                case TokenType.Modulus:
                    return Modulus(lv, rv);
                default:
                    throw ErrorFactory.CreateInvalidProgramError("Invalid multiplicative operator");
            }
        }

        public override object Walk(AdditiveExpression node)
        {
            var lv = node.LeftExpression.Accept(this);
            var rv = node.RightExpression.Accept(this);

            switch (node.Operator)
            {
                case TokenType.Plus:
                    return OpAdd(lv, rv);
                case TokenType.Minus:
                    return OpMinus(lv, rv);
                default:
                    throw ErrorFactory.CreateInvalidProgramError("Invalid additive operator");
            }
        }

        public override object Walk(Identifier node)
        {
            return Context.CurrentFrame.Resolve(node.Value);
        }

        public override object Walk(PrimitiveLiteral node)
        {
            switch (node.Type)
            {
                case TokenType.String:
                    return new BikeString(node.Value);
                case TokenType.Number:
                    return new BikeNumber(decimal.Parse(node.Value));
                case TokenType.True:
                    return new BikeBoolean(true);
                case TokenType.False:
                    return new BikeBoolean(false);
                case TokenType.Null:
                    return null;
                default:
                    throw ErrorFactory.CreateInvalidProgramError("Invalid primitive literal");
            }
        }

        public override object Walk(ExpressionStatement node)
        {
            return node.Expression.Accept(this);
        }

        public override object Walk(Expression node)
        {
            var result = new object[node.AssignmentExpressions.Count];
            for (int i = 0; i < node.AssignmentExpressions.Count; i++)
            {
                result[i] = node.AssignmentExpressions[i].Accept(this);
            }
            return result;
        }

        public override object Walk(VariableStatement node)
        {
            object result = null;
            foreach (var variableDeclaration in node.VariableDeclarations)
            {
                result = variableDeclaration.Accept(this);
            }
            return result;
        }
        #endregion
    }
}
