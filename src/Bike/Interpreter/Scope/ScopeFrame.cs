namespace Bike.Interpreter
{
    using System.Collections.Generic;
    using Builtin;
    using Parser;

    public class ScopeFrame : IScope
    {
        private readonly IDictionary<string, object> members = new Dictionary<string, object>();
        private readonly ScopeFrame global;
        private readonly ScopeFrame parent;

        internal readonly ScopeFrame Caller; 
        internal readonly BikeFunction Func;
        internal readonly SourceLocation Location;
        internal bool IsGlobal { get { return global == null; } }

        public ScopeFrame() : this (null, null, null, null, null) {} // global frame

        public ScopeFrame(BikeFunction func, ScopeFrame global, 
                          ScopeFrame parent, ScopeFrame caller)
            : this(InterpretationContext.Instance.CurrentLocation,
                   func, global, parent, caller) { }

        private ScopeFrame(SourceLocation location, BikeFunction func, 
            ScopeFrame global, ScopeFrame parent, ScopeFrame caller)
        {
            this.Location = location;
            this.Func = func;
            this.global = global;
            this.parent = parent;
            this.Caller = caller;
        }

        public void Define(string name, object value)
        {
            if (name.StartsWith("$"))
                throw ErrorFactory.CreateInvalidProgramError(
                    "Global variables must not be declared with var");
            if (members.ContainsKey(name))
                throw ErrorFactory.CreateAlreadyDefinedError(name);
            this[name] = value;
        }

        public void Assign(string name, object value)
        {
            if (name.StartsWith("$") && !IsGlobal)
                global.Assign(name, value);
            else if (members.ContainsKey(name))
                this[name] = value;
            else if (parent != null)
                parent.Assign(name, value);
            else if (name.StartsWith("$"))
                members[name] = value;
            else
                throw ErrorFactory.CreateNotDefinedError(name);
        }

        public bool Exist(string name)
        {
            if (name.StartsWith("$") && !IsGlobal)
                return global.Exist(name);
            return members.ContainsKey(name) || (parent != null && parent.Exist(name));
        }

        public object Resolve(string name)
        {
            if (name.StartsWith("$") && !IsGlobal)
                return global.Resolve(name);
            if (members.ContainsKey(name))
                return this[name];
            if (parent != null)
                return parent.Resolve(name);
            throw ErrorFactory.CreateNotDefinedError(name);
        }

        private object this[string name]
        {
            get { return members[name]; }
            set { members[name] = value; }
        }
    } 
}
