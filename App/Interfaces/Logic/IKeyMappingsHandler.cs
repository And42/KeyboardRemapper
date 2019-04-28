using System;
using System.Collections.Generic;

namespace App.Interfaces.Logic
{
    public interface IKeyMappingsHandler : IDisposable
    {
        IReadOnlyDictionary<int, int> KeyMappings { get; }

        void SetMapping(int sourceKey, int targetKey);

        bool RemoveMapping(int sourceKey);

        void RemoveAllMappings();
    }
}