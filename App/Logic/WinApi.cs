using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace App.Logic
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class WinApi
    {
        public const int WH_KEYBOARD_LL = 13; // LowLevel-hook for the keyboard
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string lpFileName);
    }
}
