using System.IO;
using System.Reflection;
using App.Interfaces.Logic.Utils;
using JetBrains.Annotations;

namespace App.Logic.Utils
{
    public class AppUtils : IAppUtils
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
