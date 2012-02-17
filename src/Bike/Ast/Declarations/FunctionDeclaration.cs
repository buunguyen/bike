namespace Bike.Ast
{
    using System.Collections.Generic;

    public partial class FunctionDeclaration : Declaration
    {
        public List<FormalParameter> FormalParameters;
        public SourceElements Body;
    }
}
