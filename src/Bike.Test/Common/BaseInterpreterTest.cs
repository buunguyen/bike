namespace Bike.Test
{
    using System;
    using Interpreter;
    using Interpreter.Builtin;
    using NUnit.Framework;
    using System.IO;

    public abstract class BaseInterpreterTest : BaseTest
    {
        private InterpretationContext context;
        private string _executingPath;

        [TestFixtureSetUp]
        public void PopulateBikeLibrary()
        {
            /* 
             * Copy all embed .bk source files to physical temporary folder 
             */

            var namespaceName = this.GetType().Namespace;
            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            _executingPath = Path.GetDirectoryName(executingAssembly.Location);

            string coreFolderPath = string.Empty;
            string filename = string.Empty;

            // Create temporary folder to put .bk source files
            coreFolderPath = Path.Combine(_executingPath, "lib/src/core");
            Directory.CreateDirectory(coreFolderPath);  

            var filenamePrefix = string.Format("{0}.lib.src.core.", namespaceName);
            foreach (var resourceName in this.GetType().Assembly.GetManifestResourceNames())
            {
                // Get the filename for physical file
                filename = resourceName.Substring(filenamePrefix.Length);
                using (var sourceFile = executingAssembly.GetManifestResourceStream(resourceName))
                using (var desFile = new FileStream(Path.Combine(coreFolderPath, filename), FileMode.CreateNew))
                {
                    sourceFile.CopyTo(desFile);
                }
            }
        }

        [TestFixtureTearDown]
        public void CleanupBikeLibrary()
        {
            try
            {
               Directory.Delete(Path.Combine(_executingPath, "lib"), true);
            }
            catch { }
        }

        [SetUp]
        protected void Initialize()
        {
            string home = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            context = InterpretationContext.StartInterpretation(home, null, null);
        }

        protected void InterpretFail(string errorType, params string[] sources)
        {
            foreach (var source in sources)
            {
                ExpectFail(errorType, source, () => Interpret(source));
            }
        }

        protected InterpretationContext Interpret(string source)
        {
            ParseAndWalk(source);
            context.Interpreter.Execute(source);
            return context;
        }

        protected void ExpectFail(string errorType, string source, Func<InterpretationContext> func)
        {
            try
            {
                func();
                Assert.Fail(source);
            }
            catch (BikeObject bo)
            {
                if (!ErrorFactory.IsErrorOfType(bo, errorType))
                {
                    Assert.Fail(source);
                }
            }
        }
    }
}
