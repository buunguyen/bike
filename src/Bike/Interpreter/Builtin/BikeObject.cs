using System.Linq;

namespace Bike.Interpreter.Builtin
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Bike.Interpreter;

    public class BikeObject : Exception, IScope, IEnumerable
    {
        internal readonly Dictionary<string, object> Members =
            new Dictionary<string, object>();

        public BikeObject Prototype
        {
            get { return (BikeObject)Members["prototype"]; }
            private set { Members["prototype"] = value; }
        }

        public BikeObject(BikeObject prototype)
        {
            Prototype = prototype;
            Members[InterpreterHelper.SpecialSuffix + "members"] = Members;
        }

        public BikeBoolean IsPrototypeOf(BikeObject other)
        {
            var prototype = other.Prototype;
            while (prototype != null && prototype != this)
                prototype = prototype.Prototype;
            return new BikeBoolean(prototype != null);
        }

        public virtual IEnumerator GetEnumerator()
        {
            return (from propName in Members.Keys 
                    where !propName.StartsWith(InterpreterHelper.SpecialSuffix) 
                    select new BikeString(propName)).GetEnumerator();
        }

        public virtual void Define(string name, object value)
        {
            Assign(name, value);
        }

        public virtual void Assign(string name, object value)
        {
            Members[name] = value;
        }

        public virtual object Resolve(string name)
        {
            var scope = FindScopeFor(name);
            if (scope == null)
			{
				bool success;
				var res = InterpretationContext.Instance.Interpreter
					.TryInvokeMemberMissing(this, name, out success);
				if (success)
					return res;
				throw ErrorFactory.CreateNotDefinedError(name);
			}
            return scope.Members[name];
        }

        public virtual bool Exist(string name)
        {
            var scope = FindScopeFor(name);
            return scope != null;
        }

        internal BikeObject FindScopeFor(string name)
        {
            var scope = this;
            while (scope != null && !scope.Members.ContainsKey(name))
                scope = scope.Prototype;
            return scope;
        }

        public virtual bool OwnExist(string name)
        {
            return Members.ContainsKey(name);
        }

        public override string ToString()
        {
            return InterpretationContext.Instance.Interpreter.Stringify(this);
        }

        public override bool Equals(object obj)
        {
            return InterpretationContext.Instance.Interpreter.OpEqual(this, obj).Value;
        }

        public override int GetHashCode()
        {
            return InterpretationContext.Instance.Interpreter.HashCode(this);
        }
    }
}
