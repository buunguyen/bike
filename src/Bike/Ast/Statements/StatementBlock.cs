namespace Bike.Ast
{
    using System.Collections.Generic;

    public partial class StatementBlock : Statement
    {
        public readonly List<Statement> Statements = new List<Statement>();
    }
}
