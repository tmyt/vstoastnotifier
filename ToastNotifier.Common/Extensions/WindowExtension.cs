using System;
using EnvDTE;
using Vsix.ToastNotifier.Interop;
#if DevEnv11
using ToastNotifier.Common;
#else
using ThreadHelperCompat = Microsoft.VisualStudio.Shell.ThreadHelper;
#endif

namespace Vsix.ToastNotifier.Extensions
{
    public static class WindowExtension
    {
        public static bool IsForegroundWindow(this Window window)
        {
            ThreadHelperCompat.ThrowIfNotOnUIThread();
            return User32.GetForegroundWindow() == (IntPtr)window.HWnd;
        }
    }
}
