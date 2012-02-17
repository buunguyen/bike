namespace Bike.Ast
{
    public partial class FormalParameter : Node
    {
        public bool IsParams;
        public Identifier Identifier;

        public override bool Equals(object obj)
        {
            var other = obj as FormalParameter;
            return other != null && 
                   other.IsParams == IsParams && 
                   other.Identifier.Equals(Identifier);
        }

        public override int GetHashCode()
        {
            return 31*IsParams.GetHashCode() + Identifier.GetHashCode();
        }
    }
}
