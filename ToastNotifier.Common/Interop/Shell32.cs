using System;
using System.Runtime.InteropServices;

namespace Vsix.ToastNotifier.Interop
{
    internal static class Shell32
    {
        [DllImport("Shell32.dll")]
        public static extern IntPtr GetCurrentProcessExplicitAppUserModelID(out IntPtr AppID);

        public static string GetCurrentProcessExplicitAppUserModelID()
        {
            IntPtr pv;
            GetCurrentProcessExplicitAppUserModelID(out pv);
            if (pv == IntPtr.Zero) return null;
            var s = Marshal.PtrToStringAuto(pv);
            Ole32.CoTaskMemFree(pv);
            return s;
        }
    }
}
