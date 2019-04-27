using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using App.Annotations;

namespace App.Logic
{
    public class HooksHandler : IDisposable
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly WinApi.LowLevelKeyboardProc _hookProc;
        private readonly IntPtr _hookPtr;
        private readonly Dictionary<int, List<Action>> _hooks = new Dictionary<int, List<Action>>();
        private readonly AtomicBoolean _disposed = new AtomicBoolean();

        public HooksHandler()
        {
            _hookProc = Hook;
            IntPtr hInstance = WinApi.LoadLibrary("user32");
            _hookPtr = WinApi.SetWindowsHookEx(WinApi.WH_KEYBOARD_LL, _hookProc, hInstance, 0);
        }

        public void AddHook(int keyCode, [NotNull] Action handler)
        {
            CheckDisposed();

            List<Action> handlers;
            if (!_hooks.TryGetValue(keyCode, out handlers))
            {
                handlers = new List<Action>();
                _hooks[keyCode] = handlers;
            }

            handlers.Add(handler);
        }

        public bool RemoveHook(int keyCode, [NotNull] Action handler)
        {
            CheckDisposed();

            return _hooks.TryGetValue(keyCode, out List<Action> handlers) && handlers.Remove(handler);
        }

        public void RemoveAllHooks()
        {
            _hooks.Clear();
        }

        public void Dispose()
        {
            if (!_disposed.CompareAndSet(false, true))
                return;

            WinApi.UnhookWindowsHookEx(_hookPtr);
        }

        private IntPtr Hook(int code, IntPtr wParam, IntPtr lParam)
        {
            bool keyDown = wParam == (IntPtr) WinApi.WM_KEYDOWN;
            bool keyUp = wParam == (IntPtr) WinApi.WM_KEYUP;

            if (code >= 0 && (keyDown || keyUp))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (_hooks.TryGetValue(vkCode, out List<Action> handlers))
                {
                    if (keyDown)
                    {
                        foreach (var handler in handlers)
                            handler();
                    }

                    return (IntPtr)1;
                }
            }

            return WinApi.CallNextHookEx(_hookPtr, code, (int)wParam, lParam);
        }

        private void CheckDisposed()
        {
            if (_disposed.Get())
                throw new ObjectDisposedException(nameof(HooksHandler));
        }
    }
}
