namespace Bike.Interpreter
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ast;
	using Builtin;

	public partial class Interpreter
	{
		public override object Walk(MemberExpression node)
		{
			return Member(node, node.Suffixes);
		}

		private object Member(MemberExpression node, IEnumerable<Node> suffixes)
		{
			var tmpSuffixes = new List<Node>(suffixes);
			var firstObject = GetFirstObjectFromMember(node.Expression, tmpSuffixes);
			return AccessOneByOne(firstObject, null, tmpSuffixes);
		}

		public override object Walk(CallExpression node)
		{ 
			return Call(node, node.Suffixes);
		}

		private object Call(CallExpression node, IEnumerable<Node> suffixes)
		{
			object firstObject;
			var memberSuffixes = new List<Node>();
			if (node.Member is MemberExpression)
			{
				var member = (MemberExpression)node.Member;
				memberSuffixes.AddRange(member.Suffixes);
				firstObject = GetFirstObjectFromMember(member.Expression, memberSuffixes);
			}
			else if (node.Member is FunctionExpression ||
					 node.Member is Identifier ||
                     node.Member is SelfExpression)
			{
				firstObject = node.Member.Accept(this);
			}
			else
			{
				throw ErrorFactory.CreateError(string.Format("Invalid member type {0}", node.Member));
			}
			var allSuffixes = new List<Node>();
			allSuffixes.AddRange(memberSuffixes);
			allSuffixes.Add(node.Arguments);
			allSuffixes.AddRange(suffixes);
			return AccessOneByOne(firstObject, null, allSuffixes);
		}

		private object GetFirstObjectFromMember(ExprNode memberExp, List<Node> suffixes)
		{
			if (memberExp is SelfExpression)
			{
				return Context.CurrentFrame.Resolve("this");
			}
			if (memberExp is Identifier)
			{
				var id = ((Identifier)memberExp).Value;
				if (Context.CurrentFrame.Exist(id))
					return Context.CurrentFrame.Resolve(id);

				// TODO handle nested type
				// propably CLR type
				if (!(suffixes[0] is TypeDescriptorSuffix) && Context.IsClrType(id))
					return Context.LoadTypeFromName(id);

				// start with namespace?  attempt further
				for (int i = 0; i < suffixes.Count; i++)
				{
					var suffix = suffixes[i];
					if (suffix is IndexSuffix)
						break;

					if (suffix is TypeDescriptorSuffix)
					{
						var typeParams = (Type[])suffix.Accept(this);
						suffixes.RemoveRange(0, i + 1);
						return Context.LoadTypeFromName(id, typeParams);
					}

					id += "." + suffix.SuffixValue(this);
					if (i < suffixes.Count - 1 && (suffixes[i + 1] is TypeDescriptorSuffix))
					{
						var typeParams = (Type[])suffixes[i + 1].Accept(this);
						suffixes.RemoveRange(0, i + 2);
						return Context.LoadTypeFromName(id, typeParams);
					}

					if (Context.IsClrType(id))
					{
						suffixes.RemoveRange(0, i + 1);
						return Context.LoadTypeFromName(id);
					}
				}

                if (id.IndexOf('.') >= 0)
                    id = id.Substring(0, id.IndexOf('.'));
				throw ErrorFactory.CreateNotDefinedError(id);
			}
			return memberExp.Accept(this);
		}

		private object AccessOneByOne(object currentTarget, object previousTarget, List<Node> suffixes)
		{
			if (suffixes.Count == 0)
				return currentTarget;

			if (currentTarget == null)
				throw ErrorFactory.CreateNullError("Object");

			var currentSuffix = suffixes[0];

			// bike function, .NET ctor or .NET delegate
			if (currentSuffix is Arguments)
			{
				var arguments = (Arguments) currentSuffix;
				// bike function
				if (currentTarget is BikeFunction)
				{
					previousTarget = CallBikeFunction((BikeFunction)currentTarget,
													  previousTarget, arguments);
				}
				// ctor
				else if (currentTarget is Type)
				{
					var argValues = GetArgumentValues(arguments);
					previousTarget = CreateInstance((Type)currentTarget, argValues);
				}
				// delegate
				else
				{
					var args = GetArgumentValues(arguments);
					previousTarget = CallDelegate((Delegate)currentTarget, args);
				}
				return AccessOneByOne(previousTarget, currentTarget, suffixes.ExceptFirst());
			}
			if (currentSuffix is PropertyReferenceSuffix || 
				currentSuffix is IndexSuffix || 
				currentSuffix is TypeDescriptorSuffix)
			{
				if (currentTarget is BikeObject)
				{
					var currentSuffixStr = currentSuffix.SuffixValue(this);
					previousTarget = ((BikeObject)currentTarget).Resolve(currentSuffixStr);
				}
				else if (currentTarget is Type) // Static context
				{
					var currentSuffixStr = currentSuffix.SuffixValue(this);

					// Static generic call
					if (suffixes.Count > 2 &&
						suffixes[1] is TypeDescriptorSuffix &&
						suffixes[2] is Arguments
						)
					{
						var typeParams = (Type[])suffixes[1].Accept(this);
						var args = GetArgumentValues((Arguments)suffixes[2]);
						previousTarget = CallStaticFunction((Type)currentTarget, typeParams, currentSuffixStr, args);
						suffixes = suffixes.ExceptFirst().ExceptFirst();
					}

					// Static method or delegate
					else if (suffixes.Count > 1 && suffixes[1] is Arguments)
					{
						try
						{
							// delegate?
							previousTarget = GetStaticProperty((Type)currentTarget, currentSuffixStr);
						}
						catch (BikeObject bo)
						{
							if (!ErrorFactory.IsClrError(bo))
								throw;
							// method?  invoke immediately to avoid saving MethodInfo
							var args = GetArgumentValues((Arguments)suffixes[1]);
							previousTarget = CallStaticFunction((Type)currentTarget, currentSuffixStr, args);
							suffixes = suffixes.ExceptFirst();
						}
					}
					else // .NET static property, invoke immediately
					{
						previousTarget = GetStaticProperty((Type)currentTarget, currentSuffixStr);
					}
				}

				// Must be .NET generic instance method
				else if (suffixes.Count > 2 &&
						suffixes[1] is TypeDescriptorSuffix &&
						suffixes[2] is Arguments)
				{
					var currentSuffixStr = currentSuffix.SuffixValue(this);
					var typeParams = (Type[])suffixes[1].Accept(this);
					var args = GetArgumentValues((Arguments)suffixes[2]);
					previousTarget = CallInstanceFunction(currentTarget, typeParams, currentSuffixStr, args);
					suffixes = suffixes.ExceptFirst().ExceptFirst();
				}

				// .NET instance method or delegate
				else if (suffixes.Count > 1 && suffixes[1] is Arguments)
				{
					try
					{
						// delegate?
						var currentSuffixStr = currentSuffix.SuffixValue(this);
						previousTarget = GetInstanceProperty(currentTarget, currentSuffixStr);
					}
					catch (BikeObject bo)
					{
						if (!ErrorFactory.IsClrError(bo))
							throw;
						// method?
						var currentSuffixStr = currentSuffix.SuffixValue(this);
						var args = GetArgumentValues((Arguments)suffixes[1]);
						previousTarget = CallInstanceFunction(currentTarget, currentSuffixStr, args);
						suffixes = suffixes.ExceptFirst();
					}
				}

                // .NET instance property
				else if (currentSuffix is PropertyReferenceSuffix) 
				{
					var currentSuffixStr = currentSuffix.SuffixValue(this);
					previousTarget = GetInstanceProperty(currentTarget, currentSuffixStr);
				}

                // .NET indexer
				else 
				{
					previousTarget = GetInstanceIndexer(currentTarget, currentSuffix.Arguments(this));
				}
				return AccessOneByOne(previousTarget, currentTarget, suffixes.ExceptFirst());
			}
			throw ErrorFactory.CreateError(string.Format("Invalid suffix type {0}", currentSuffix));
		}

		public object TryInvokeMemberMissing (BikeObject target, string name, out bool success)
		{
			var scope = target.FindScopeFor(InterpreterHelper.MemberMissing);
			if (scope == null)
			{
				success = false;
				return null;
			}
			var memberMissingFunc = scope.Members[InterpreterHelper.MemberMissing] as BikeFunction;
			if (memberMissingFunc == null)
				throw ErrorFactory.CreateTypeError(InterpreterHelper.MemberMissing + " must be a function");
			success = true;
			return CallBikeFunction(memberMissingFunc, scope, new[] { new BikeString(name) });
		}

		public object CallBikeFunction(BikeFunction function, object self, Arguments arguments)
		{
			var argValues = GetArgumentValues(arguments);
			return CallBikeFunction(function, self, argValues);
		}

		public object CallBikeFunction(BikeFunction function, object self, object[] argValues)
		{
			var parameters = function.Parameters;
			Action<ScopeFrame> bindArgs = scopeFrame =>
			{
				scopeFrame.Define("this", self);
				for (var i = 0; i < parameters.Count; i++)
				{
					var argName = parameters[i].Identifier.Value;
					if (i < argValues.Length) // normal parameter
					{
						if (parameters[i].IsParams) // collapse
						{
							var bk = new BikeArray();
							for (var j = i; j < argValues.Length; j++)
							{
								bk[j - i] = argValues[j];
							}
							scopeFrame.Define(argName, bk);
						}
						else
						{
							scopeFrame.Define(argName, argValues[i]);
						}
					}
					else // optional parameter
					{
						scopeFrame.Define(argName, null);
					}
				}
			};
			Func<object> execute = () =>
			{
				try
				{
					return function.Body.Accept(this);
				}
				catch (Return r)
				{
					return r.Value;
				}
			};
			return Context.OpenScopeFor(execute,
										withInit: bindArgs,
										parentScope: function.BoundScope,
										func: function);
		}


		private object[] GetArgumentValues(Arguments arguments)
		{
			var values = new List<object>();
			foreach (var argument in arguments.Children)
			{
				var value = argument.Expression.Accept(this);
				if (argument.ShouldExpand)
				{
					if (value == null)
					{
						values.Add(null);
					}
					else if (value is BikeArray)
					{
						values.AddRange(((BikeArray)value).Cast<object>());
					}
					else
					{
						throw ErrorFactory.CreateTypeError("Can only expand arrays");
					}
				}
				else
				{
					values.Add(value);
				}
			}
			return values.ToArray();
		}
	}
}
