using System;
using System.Collections.Generic;
using System.Linq;
using WindowsInput;
using WindowsInput.Native;
using App.Interfaces.Logic;
using JetBrains.Annotations;

namespace App.Logic
{
    public class KeyMappingsHandler : IKeyMappingsHandler
    {
        public IReadOnlyDictionary<int, int> KeyMappings => _keyMappings;

        [NotNull] private readonly IAppSettings _appSettings;
        [NotNull] private readonly IHooksHandler _hooksHandler;

        private readonly IInputSimulator _inputSimulator = new InputSimulator();
        private readonly Dictionary<int, int> _keyMappings = new Dictionary<int, int>();
        private readonly AtomicBoolean _disposed = new AtomicBoolean();

        public KeyMappingsHandler(
            [NotNull] IAppSettings appSettings,
            [NotNull] IHooksHandler hooksHandler
        )
        {
            _appSettings = appSettings;
            _hooksHandler = hooksHandler;

            if (appSettings.KeyMappings != null)
            {
                foreach (var mapping in appSettings.KeyMappings)
                {
                    hooksHandler.SetHook(mapping.Key, CreateMappingHandler(mapping.Value));
                    _keyMappings.Add(mapping.Key, mapping.Value);
                }
            }
        }

        public void SetMapping(int sourceKey, int targetKey)
        {
            CheckDisposed();

            _keyMappings[sourceKey] = targetKey;
            _hooksHandler.SetHook(sourceKey, CreateMappingHandler(targetKey));
            UpdateSettings();
        }

        public bool RemoveMapping(int sourceKey)
        {
            CheckDisposed();

            if (!_keyMappings.Remove(sourceKey))
                return false;

            _hooksHandler.RemoveHook(sourceKey);
            UpdateSettings();

            return true;
        }

        public void RemoveAllMappings()
        {
            CheckDisposed();

            _keyMappings.Clear();
            _hooksHandler.RemoveAllHooks();
            UpdateSettings();
        }

        public void Dispose()
        {
            if (!_disposed.CompareAndSet(false, true))
                return;

            _hooksHandler.Dispose();
        }

        private void UpdateSettings()
        {
            _appSettings.KeyMappings = KeyMappings.ToDictionary(it => it.Key, it => it.Value);
        }

        private void CheckDisposed()
        {
            if (_disposed.Get())
                throw new ObjectDisposedException(nameof(KeyMappingsHandler));
        }

        private Action CreateMappingHandler(int keyCode)
        {
            return () => _inputSimulator.Keyboard.KeyPress((VirtualKeyCode)keyCode);
        }
    }
}
