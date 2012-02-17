namespace Bike.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Interpreter;
    using NDesk.Options;

    internal class Program
    {
        private static void Main(string[] args)
        {
            string homePath = null;
            string libPaths = null;
            var p = new OptionSet
                        {
                            {
                                "h|home=", "(optional) The path to Bike's installation folder.  If not specified, BIKE_HOME env variable must exist and will be used.",
                                v => homePath = v 
                            },
                            {
                                "l|libs=", "(optional) Semi-colon separated list of paths to extra library folders.  If not specified, BIKE_LIBS env variable will be used if it exists.",
                                v => libPaths = v 
                            },
                        };
            
            List<string> extras;
            try
            {
                extras = p.Parse(args);
                if (homePath == null)
                {
                    homePath = Environment.GetEnvironmentVariable("BIKE_HOME");
                }
                if (libPaths == null)
                {
                    libPaths = Environment.GetEnvironmentVariable("BIKE_LIBS");
                }
                if (homePath == null || extras.Count != 1)
                {
                    ShowHelp(p);
                    return;
                }
            }
            catch (OptionException)
            {
                ShowHelp(p);
                return;
            }
            var bikeFile = extras[0];
            
            var fullPath = bikeFile;
            if (!Path.IsPathRooted(fullPath))
                fullPath = Path.Combine(Environment.CurrentDirectory, fullPath);
            try
            {
                Hosting.Engine.Run(homePath, libPaths, fullPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(InterpretationContext.Instance.
                    Interpreter.Stringify(e, true));
                Console.Read();
            } 
        }

        private static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: bike [OPTIONS]+ bile_file");
            Console.WriteLine("Execute a bike source file");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}