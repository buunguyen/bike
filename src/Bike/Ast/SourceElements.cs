namespace Bike.Ast
{
    using System.Collections.Generic;

    public partial class SourceElements : Node
    {
        public readonly List<Statement> Statements = new List<Statement>();
    }
}
