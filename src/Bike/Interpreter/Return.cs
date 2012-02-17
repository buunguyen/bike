namespace Bike.Interpreter
{
    class Return : ControlFlow
    {
        public readonly object Value;

        public Return(object value)
        {
            Value = value;
        }
    }
}
