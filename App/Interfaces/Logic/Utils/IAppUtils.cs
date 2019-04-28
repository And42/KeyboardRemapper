using JetBrains.Annotations;

namespace App.Interfaces.Logic.Utils
{
    public interface IAppUtils
    {
        [NotNull]
        string GetExecutablePath();

        [CanBeNull]
        string GetExecutableDir();
    }
}