using System;
using System.Collections.Generic;
using System.Linq;
using WindowsInput;
using WindowsInput.Native;
using App.Annotations;

namespace App.Logic
{
    public class KeyMappingsHandler : IDisposable
    {
        public IReadOnlyDictionary<int, int> KeyMappings => _keyMappings;

        [NotNull] private readonly AppSettings _appSettings;
        [NotNull] private readonly HooksHandler _hooksHandler;

        private readonly IInputSimulator _inputSimulator = new InputSimulator();
        private readonly Dictionary<int, int> _keyMappings = new Dictionary<int, int>();
        private readonly AtomicBoolean _disposed = new AtomicBoolean();

        public KeyMappingsHandler(
            [NotNull] AppSettings appSettings,
            [NotNull] HooksHandler hooksHandler
        )
        {
            _appSettings = appSettings;
            _hooksHandler = hooksHandler;

            if (appSettings.KeyMappings != null)
            {
                foreach (var mapping in appSettings.KeyMappings)
                {
                    hooksHandler.AddHook(mapping.Key, CreateMappingHandler(mapping.Value));
                    _keyMappings.Add(mapping.Key, mapping.Value);
                }
            }
        }

        public void AddMapping(int sourceKey, int targetKey)
        {
            CheckDisposed();

            _keyMappings.Add(sourceKey, targetKey);
            _hooksHandler.AddHook(sourceKey, CreateMappingHandler(targetKey));
            UpdateSettings();
        }

        public void RemoveMapping(int sourceKey)
        {
            throw new NotImplementedException();

            CheckDisposed();

            _keyMappings.Remove(sourceKey);
            UpdateSettings();
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
