namespace Bike.Interpreter
{
    using System;
    using System.IO;
    using System.Threading;
    using Ast;
    using Builtin;
    using Parser;

    public class InterpretationContext
    {
        public static readonly BikeObject ObjectBase = new BikeObject(null);
        public static readonly BikeObject FunctionBase = new BikeObject(ObjectBase);
        public static readonly BikeObject ArrayBase = new BikeObject(ObjectBase);
        public static readonly BikeObject StringBase = new BikeObject(ObjectBase);
        public static readonly BikeObject NumberBase = new BikeObject(ObjectBase);
        public static readonly BikeObject BooleanBase = new BikeObject(ObjectBase);
        public static readonly BikeObject ErrorBase = new BikeObject(ObjectBase);
        
        private static readonly ThreadLocal<InterpretationContext> LocalInstance 
            = new ThreadLocal<InterpretationContext>();
        public static InterpretationContext Instance
        {
            get { return LocalInstance.Value; }
            set { LocalInstance.Value = value; }
        }

        public readonly Interpreter Interpreter; 
        private readonly ScopeStack scopeStack; 
        private readonly ClrImportContext clrImportContext;  
        private readonly BikeImportContext bikeImportContext;

        public volatile string CurrentExecFolder;
        public volatile SourceLocation CurrentLocation;
        public volatile ScopeFrame LastFrame;

        public static InterpretationContext StartInterpretation(string homePath, string addonLibFolders, string filePath)
        {
            var coreLibFolder = Path.Combine(homePath, "lib/src");
            var context = new InterpretationContext(coreLibFolder, addonLibFolders);
            context.Interpreter.Run("core.bk", filePath);
            return context;
        }

        private InterpretationContext(string coreLibFolder, string addonLibFolders)
            : this(new ScopeStack(InitScopeFrame()), 
                   new ClrImportContext(coreLibFolder, addonLibFolders),
                   new BikeImportContext(coreLibFolder, addonLibFolders))
        {
        }

        internal InterpretationContext(InterpretationContext otherContext)
            : this (new ScopeStack(otherContext.scopeStack), 
                    otherContext.clrImportContext,
                    new BikeImportContext(otherContext.bikeImportContext))
        {
            CurrentExecFolder = otherContext.CurrentExecFolder; 
            CurrentLocation = otherContext.CurrentLocation;
            LastFrame = otherContext.LastFrame; 
        }

        private InterpretationContext(ScopeStack scopeStack, 
            ClrImportContext clrImportContext, BikeImportContext bikeImportContext)
        {
            Instance = this;
            this.scopeStack = scopeStack;
            this.clrImportContext = clrImportContext;
            this.bikeImportContext = bikeImportContext;
            Interpreter = new Interpreter(this);
        }

        private static ScopeFrame InitScopeFrame()
        {
            var scopeFrame = new ScopeFrame();
            var bikeNamespace = new BikeObject(ObjectBase);
            scopeFrame.Define("Bike", bikeNamespace);
            bikeNamespace.Define("Object", ObjectBase);
            bikeNamespace.Define("Array", ArrayBase);
            bikeNamespace.Define("String", StringBase);
            bikeNamespace.Define("Number", NumberBase);
            bikeNamespace.Define("Boolean", BooleanBase);
            bikeNamespace.Define("Function", FunctionBase);
            bikeNamespace.Define("Error", ErrorBase);
            return scopeFrame;
        }

        #region Scope
        public ScopeFrame CurrentFrame
        {
            get { return scopeStack.CurrentFrame; }
        }

        public ScopeFrame GlobalFrame
        {
            get { return scopeStack.GlobalFrame; }
        }

        public T OpenScopeFor<T>(Func<T> body,
            bool when = true,
            Action<ScopeFrame> withInit = null,
            ScopeFrame parentScope = null,
            BikeFunction func = null)
        {
            return scopeStack.OpenScopeFor(body, when, withInit, parentScope, func);
        }

        public void OpenScopeFor(Action body,
            bool when = true,
            Action<ScopeFrame> withInit = null,
            ScopeFrame parentScope = null,
            BikeFunction func = null)
        {
            scopeStack.OpenScopeFor(body, when, withInit, parentScope, func);
        }
        #endregion

        #region Import
        public string ImportFile(string path, out bool alreadyImport)
        {
            return bikeImportContext.ImportBikeFile(CurrentExecFolder, path, out alreadyImport);
        }

        public void ImportAssembly(string assemblyString)
        {
            clrImportContext.ImportAssembly(CurrentExecFolder, assemblyString);
        }

        public Type LoadTypeFromName(string typeName)
        {
            return clrImportContext.LoadTypeFromName(typeName);
        }

        public Type LoadTypeFromName(string typeName, Type[] typeParams)
        {
            return clrImportContext.LoadTypeFromName(typeName, typeParams);
        }

        public bool IsClrType(string typeName)
        {
            return clrImportContext.IsVisibleClrType(typeName);
        }

        public Type LoadTypeFromDescriptor(TypeDescriptor typeDescriptor)
        {
            return clrImportContext.LoadTypeFromDescriptor(typeDescriptor);
        }
        #endregion
    }
}
