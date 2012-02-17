namespace Bike.Ast
{
    using System.Collections.Generic;

    public partial class TypeDescriptor : Node
    {
        public string Name;
        public readonly List<TypeDescriptor> TypeDescriptors = new List<TypeDescriptor>();
    }
}
