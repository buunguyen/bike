namespace Bike.Ast
{
    using System.Collections.Generic;

    public partial class TypeDescriptorSuffix : Node
    {
        public readonly List<TypeDescriptor> TypeDescriptors = new  List<TypeDescriptor>();
    }
}
