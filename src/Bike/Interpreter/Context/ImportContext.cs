namespace Bike.Interpreter
{
    using System.Collections.Generic;
    using System.IO;
    using Builtin;

    internal abstract class ImportContext
    {
        protected readonly string CoreLibFolder;
        protected readonly string AddonLibFolders;
        protected readonly List<string> SearchPaths = new List<string>();

        protected ImportContext(string coreLibFolder, string addonLibFolders)
        {
            CoreLibFolder = coreLibFolder;
            AddonLibFolders = addonLibFolders;
            SearchPaths.Add(coreLibFolder);
            if (string.IsNullOrWhiteSpace(addonLibFolders)) 
                return;
            string[] paths = addonLibFolders.Split(Path.PathSeparator);
            foreach (var path in paths)
                if (Directory.Exists(path))
                    SearchPaths.Add(path);
            SearchPaths.Reverse();
        }

        protected string ResolvePath(string currentFolder, string filePath)
        {
            if (Path.IsPathRooted(filePath))
            {
                if (File.Exists(filePath))
                    return filePath;
                throw ErrorFactory.CreateLoadError(filePath);
            }
            if (currentFolder != null)
            {
                var path = SearchInFolder(currentFolder, filePath);
                if (path != null)
                    return path;
            }
            foreach (string libFolder in SearchPaths)
            {
                var path = SearchInFolder(libFolder, filePath);
                if (path != null)
                    return path;
            }
            throw ErrorFactory.CreateLoadError(filePath);
        }

        private static string SearchInFolder(string folder, string filePath)
        {
            var path = Path.Combine(folder, filePath);
            if (File.Exists(path))
                return path;
            foreach (var dir in new DirectoryInfo(folder).GetDirectories())
            {
                path = SearchInFolder(dir.FullName, filePath);
                if (path != null)
                    return path;
            }
            return null;
        }
    }
}
