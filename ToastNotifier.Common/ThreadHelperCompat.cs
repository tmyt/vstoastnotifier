using Microsoft.VisualStudio.Shell;

namespace ToastNotifier.Common
{
    internal class ThreadHelperCompat
    {
        public static void ThrowIfNotOnUIThread()
        {
#if DevEnv11
            return;
#else
            ThreadHelper.ThrowIfNotOnUIThread();
#endif
        }
    }
}
