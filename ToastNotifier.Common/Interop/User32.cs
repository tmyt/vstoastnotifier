using System;
using System.Runtime.InteropServices;

namespace Vsix.ToastNotifier.Interop
{
    internal static class User32
    {
        /// <summary>The GetForegroundWindow function returns a handle to the foreground window.</summary>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
    }
}
