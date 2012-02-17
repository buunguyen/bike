namespace Bike.Interpreter.Builtin
{
    public class LoadError : Error
    {
        public LoadError(string path)
            : base(string.Format("Cannot load path '{0}'", path), 
                   ResolvePrototype("LoadError"))
        {
            Members["path"] = new BikeString(path);
        }
    }
}
