using System.IO;
using System.Reflection;
using JetBrains.Annotations;

namespace App.Logic.Utils
{
    public class AppUtils
    {
        [NotNull]
        public string GetExecutablePath()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        [CanBeNull]
        public string GetExecutableDir()
        {
            return Path.GetDirectoryName(GetExecutablePath());
        }
    }
}
