namespace Bike.Hosting
{
    using Interpreter;

    public class Engine
    {
        public static ScopeFrame Run(string homePath, string libPaths, string filePath)
        {
            var context = InterpretationContext.StartInterpretation(homePath, libPaths, filePath);
            return context.GlobalFrame;
        }
    }
}
