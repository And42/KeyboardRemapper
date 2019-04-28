using System;
using JetBrains.Annotations;

namespace App.Interfaces.Logic
{
    public interface IHooksHandler : IDisposable
    {
        void SetAllKeysHandler([CanBeNull] Func<int, bool> keyHandler);

        void SetHook(int keyCode, [NotNull] Action handler);

        bool RemoveHook(int keyCode);

        void RemoveAllHooks();
    }
}