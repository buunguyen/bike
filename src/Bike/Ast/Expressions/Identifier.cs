namespace Bike.Ast
{
    public partial class Identifier : ExprNode
    {
        public string Value { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Identifier))
                return false;
            return Value.Equals(((Identifier)obj).Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
