namespace Bike.Installer.Custom
{
    using System.IO;
    using System;
    using System.Collections;
    using System.ComponentModel;

    [RunInstaller(true)]
    public partial class BikeInstaller : System.Configuration.Install.Installer
    {
        public BikeInstaller()
        {
            InitializeComponent();
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            Environment.SetEnvironmentVariable("BIKE_HOME", null, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("BIKE_HOME", GetHomeFolder(), EnvironmentVariableTarget.User);
            var paths = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
            var bin = GetBinFolder();
            if (string.IsNullOrEmpty(paths))
            {
                Environment.SetEnvironmentVariable("PATH", bin, EnvironmentVariableTarget.User);
            }
            else if (!paths.ToUpperInvariant().Contains(bin.ToUpperInvariant()))
            {
                paths = paths + (paths.EndsWith(";") ? "" : ";") + bin;
                Environment.SetEnvironmentVariable("PATH", paths, EnvironmentVariableTarget.User);
            }
            //Win32.BroadCastSettingChange();
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            Cleanup();
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState)
        {
            Cleanup();
        }

        private static void Cleanup()
        {
            Environment.SetEnvironmentVariable("BIKE_HOME", null, EnvironmentVariableTarget.User);
            var paths = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(paths))
                return;
            var bin = GetBinFolder();
            var index = paths.ToUpperInvariant().IndexOf(bin.ToUpperInvariant());
            if (index >= 0)
            {   // a;b;c(;)
                paths = paths.Substring(0, index) +
                        paths.Substring(index + bin.Length);
                paths = paths.Replace(";;", ";");
                if (paths.StartsWith(";"))
                    paths = paths.Substring(1);
                Environment.SetEnvironmentVariable("PATH", paths, EnvironmentVariableTarget.User);
            }
        }

        private static string GetHomeFolder()
        {
            var path = GetBinFolder();
            var dir = Directory.GetParent(path);
            return dir.FullName;
        }

        private static string GetBinFolder()
        {
            var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(path);
        }
    }
}
