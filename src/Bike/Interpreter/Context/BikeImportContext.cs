namespace Bike.Interpreter
{
    using System.Collections.Generic;

    internal class BikeImportContext : ImportContext
    {
        private readonly object syncLock = new object();
        private readonly ISet<string> importedBikeFiles;

        public BikeImportContext(string coreLibFolder, string addonLibFolders)
            : base(coreLibFolder, addonLibFolders)
        {
            importedBikeFiles = new HashSet<string>();
        }

        public BikeImportContext(BikeImportContext other)
            : base(other.CoreLibFolder, other.AddonLibFolders)
        {
            lock (other.syncLock)
            {
                importedBikeFiles = new HashSet<string>(other.importedBikeFiles);
            }
        }

        public string ImportBikeFile(string currentFolder, string filePath, out bool alreadyImported)
        {
            var fullPath = ResolvePath(currentFolder, filePath);
            return ImportBikeFile(fullPath, out alreadyImported);
        }

        private string ImportBikeFile(string filePath, out bool alreadyImported)
        {
            lock (syncLock)
            {
                alreadyImported = true;
                if (!importedBikeFiles.Contains(filePath))
                {
                    importedBikeFiles.Add(filePath);
                    alreadyImported = false;
                }
                return filePath;
            }
        }
    }
}
