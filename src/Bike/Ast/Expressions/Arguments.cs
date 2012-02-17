namespace Bike.Ast
{
    using System.Collections.Generic;

    public partial class Arguments : Node
    {
        public readonly List<Argument> Children = new List<Argument>();
    }
}
