namespace Bike.Interpreter
{
    public interface IScope
    {
        void Define(string name, object value);
        void Assign(string name, object value);
        object Resolve(string name);
        bool Exist(string name);
    }
}
