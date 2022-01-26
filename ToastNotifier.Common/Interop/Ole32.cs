using System;
using System.Runtime.InteropServices;

namespace Vsix.ToastNotifier.Interop
{
    internal static class Ole32
    {
        [DllImport("ole32.dll")]
        public static extern void CoTaskMemFree(IntPtr pv);
    }
}
