using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using App.Interfaces.Logic;
using JetBrains.Annotations;

namespace App.Logic
{
    public class HooksHandler : IHooksHandler
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly WinApi.LowLevelKeyboardProc _hookProc;
        private readonly IntPtr _hookPtr;
        private readonly Dictionary<int, Action> _hooks = new Dictionary<int, Action>();
        private readonly AtomicBoolean _disposed = new AtomicBoolean();

        [CanBeNull]
        private Func<int, bool> _allKeysHandler;

        public HooksHandler()
        {
            _hookProc = Hook;
            IntPtr hInstance = WinApi.LoadLibrary("user32");
            _hookPtr = WinApi.SetWindowsHookEx(WinApi.WH_KEYBOARD_LL, _hookProc, hInstance, 0);
        }

        public void SetAllKeysHandler([CanBeNull] Func<int, bool> keyHandler)
        {
            _allKeysHandler = keyHandler;
        }

        public void SetHook(int keyCode, [NotNull] Action handler)
        {
            CheckDisposed();

            _hooks[keyCode] = handler;
        }

        public bool RemoveHook(int keyCode)
        {
            CheckDisposed();

            return _hooks.Remove(keyCode);
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

                if (_allKeysHandler?.Invoke(vkCode) == true)
                    return (IntPtr) 1;

                if (_hooks.TryGetValue(vkCode, out Action handler))
                {
                    if (keyDown)
                        handler();

                    return (IntPtr) 1;
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
