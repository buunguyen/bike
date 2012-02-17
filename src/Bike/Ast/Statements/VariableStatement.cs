namespace Bike.Ast
{
    using System.Collections.Generic;

    public partial class VariableStatement : Statement
    {
        public readonly List<VariableDeclaration> VariableDeclarations = 
            new List<VariableDeclaration>();
    }
}
