
// Generated code
namespace Bike.Ast {
    using System;
    using Parser;
    using Interpreter;
    using Interpreter.Builtin;
	
	partial class Node { 
		public abstract object Accept(IWalker walker);
	}
		
	partial class SourceElements { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class SourceUnitTree { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class FormalParameter { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class FunctionDeclaration { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class VariableDeclaration { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class AdditiveExpression { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class AndExpression { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class Argument { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class Arguments { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class CallExpression { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class ConditionalExpression { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class EqualityExpression { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class ExecExpression { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class Expression { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class FunctionExpression { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class Identifier { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class IndexSuffix { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class LeftAssignmentExpression { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class MemberExpression { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class MultiplicativeExpression { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class OrExpression { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class PropertyReferenceSuffix { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class RelationalExpression { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class SelfExpression { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class TypeDescriptor { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class TypeDescriptorSuffix { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class UnaryExpression { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class ArrayLiteral { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class ObjectLiteral { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class PrimitiveLiteral { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class BreakStatement { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class CaseClause { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class EmptyStatement { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class ExpressionStatement { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class ForInStatement { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class IfStatement { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class LoadStatement { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class NextStatement { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class RescueClause { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class ReturnStatement { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class StatementBlock { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class SwitchStatement { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class ThrowStatement { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class TryStatement { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class VariableStatement { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
		
	partial class WhileStatement { 
		public override object Accept(IWalker walker) {
			if (InterpretationContext.Instance != null)
				InterpretationContext.Instance.CurrentLocation = Token.Location;
			try {
				return walker.Walk(this);
            }
            catch (Exception e)
            {
                if (e is ParseException || 
					e is BikeObject ||
					e is ControlFlow)
                    throw;
                // throw;
				throw ErrorFactory.CreateClrError(e);
            }
		}
	}
	}