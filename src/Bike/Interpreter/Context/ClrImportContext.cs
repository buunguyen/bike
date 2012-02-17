namespace Bike.Interpreter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Ast;
    using Builtin;

    internal class ClrImportContext : ImportContext
    {
        private static readonly ISet<Assembly> StartupAssemblies = new HashSet<Assembly>();

        private readonly object syncLock = new object();
        private readonly ISet<Assembly> importedAssemblies = new HashSet<Assembly>();
        private readonly IDictionary<string, Type> visibleTypes = new Dictionary<string, Type>();

        static ClrImportContext()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                StartupAssemblies.Add(assembly);
            }
        }

        public ClrImportContext(string coreLibFolder, string addonLibFolders)
            : base(coreLibFolder, addonLibFolders)
        {
            foreach (var assembly in StartupAssemblies)
                ImportAssembly(assembly);
        }

        public void ImportAssembly(string currentFolder, string assemblyString)
        {
            var assembly = ResolveAssembly(currentFolder, assemblyString);
            lock (syncLock)
            {
                ImportAssembly(assembly);
            }
        }

        private void ImportAssembly(Assembly assembly)
        {
            if (importedAssemblies.Contains(assembly))
                return;
            importedAssemblies.Add(assembly);
            ImportTypes(assembly.GetTypes());
        }

        private void ImportTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (!visibleTypes.ContainsKey(type.FullName))
                    visibleTypes.Add(type.FullName, type);
                ImportTypes(type.GetNestedTypes());
            }
        }

        public bool IsVisibleClrType(string typeName)
        {
            lock (syncLock)
            {
                return visibleTypes.ContainsKey(typeName);
            }
        }

        public Type LoadTypeFromName(string typeName)
        {
            lock (syncLock)
            {
                if (!IsVisibleClrType(typeName))
                    throw ErrorFactory.CreateClrError(string.Format("Type {0} cannot be found", typeName));
                return visibleTypes[typeName];
            }
        }

        public Type LoadTypeFromName(string typeName, Type[] typeParams)
        {
            var sb = new StringBuilder();
            sb.Append(typeName)
                .Append("`")
                .Append(typeParams.Length);
            var openType = LoadTypeFromName(sb.ToString());
            return openType.MakeGenericType(typeParams);
        }

        public Type LoadTypeFromDescriptor(TypeDescriptor typeDescriptor)
        {
            if (typeDescriptor.TypeDescriptors.Count == 0)
                return LoadTypeFromName(typeDescriptor.Name);
            var typeParams = typeDescriptor
                .TypeDescriptors
                .Select(LoadTypeFromDescriptor)
                .ToArray();
            return LoadTypeFromName(typeDescriptor.Name, typeParams);
        }

        private Assembly ResolveAssembly(string currentFolder, string assemblyString)
        {
            if (Path.GetExtension(assemblyString).ToUpperInvariant() == ".DLL")
            {
                var fullPath = ResolvePath(currentFolder, assemblyString);
                return Assembly.LoadFrom(fullPath);
            }
            return Assembly.Load(assemblyString);
        }
    }
}
