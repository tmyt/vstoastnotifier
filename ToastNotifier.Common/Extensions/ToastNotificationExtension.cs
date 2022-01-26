using Windows.UI.Notifications;
using Vsix.ToastNotifier.Interop;
using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
#if DevEnv11
using ToastNotifier.Common;
#else
using ThreadHelperCompat = Microsoft.VisualStudio.Shell.ThreadHelper;
#endif

namespace Vsix.ToastNotifier.Extensions
{
    public static class ToastNotificationExtension
    {
        private static string ApplicationID = "VisualStudio.";
        private static readonly Lazy<Windows.UI.Notifications.ToastNotifier> Notifier = new Lazy<Windows.UI.Notifications.ToastNotifier>(() =>
        {
            ThreadHelperCompat.ThrowIfNotOnUIThread();
            var dte = (DTE)Package.GetGlobalService(typeof(DTE));
            return ToastNotificationManager.CreateToastNotifier(EditionToAppUserModelId(dte.Edition, dte.Version));
        });

        public static void Show(this ToastNotification notification)
        {
            ThreadHelperCompat.ThrowIfNotOnUIThread();
            // Suppress toast in foreground
            var dte = (DTE)Package.GetGlobalService(typeof(DTE));
            if (dte.MainWindow.IsForegroundWindow()) return;
            // Show toast
            Notifier.Value.Show(notification);
        }

        /// <summary>
        /// Get AppUserModelId for each editions
        /// </summary>
        /// <param name="edition">Value of DTE.Edition</param>
        /// <param name="version"></param>
        /// <returns>Correspond AppUserModelId for current process</returns>
        private static string EditionToAppUserModelId(string edition, string version)
        {
            switch (edition)
            {
                case "WD Express":
                    return "VWDExpress." + version;
                case "Desktop Express":
                    return "WDExpress." + version;
                case "VSWin Express":
                    return "VSWinExpress." + version;
                case "PD Express":
                    return "VPDExpress." + version;
            }
            // detect AppUserModelId
            var s = Shell32.GetCurrentProcessExplicitAppUserModelID();
            return !string.IsNullOrEmpty(s) ? s : ApplicationID + version;
        }
    }
}
