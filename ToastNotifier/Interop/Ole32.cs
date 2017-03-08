using System;
using System.Runtime.InteropServices;

namespace Company.ToastNotifier.Interop
{
    static class Ole32
    {
        [DllImport("ole32.dll")]
        public static extern void CoTaskMemFree(IntPtr pv);
    }
}
