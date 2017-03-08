using System;
using System.Runtime.InteropServices;

namespace Company.ToastNotifier.Interop
{
    static class User32
    {
        /// <summary>The GetForegroundWindow function returns a handle to the foreground window.</summary>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
    }
}
