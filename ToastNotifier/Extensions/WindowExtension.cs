using System;
using EnvDTE;
using Vsix.ToastNotifier.Interop;

namespace Vsix.ToastNotifier.Extensions
{
    public static class WindowExtension
    {
        public static bool IsForegroundWindow(this Window window)
        {
            return User32.GetForegroundWindow() == (IntPtr)window.HWnd;
        }
    }
}
