using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.AsyncPackageHelpers;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Vsix.ToastNotifier.Extensions;
using Vsix.ToastNotifier.Properties;

namespace Vsix.ToastNotifier.Product
{
    /// <inheritdoc />
    [AsyncPackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(Guids.ToastNotifierPackage)]
    [Microsoft.VisualStudio.AsyncPackageHelpers.ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class ToastNotifierPackage : Package, IAsyncLoadablePackageInitialize
    {
        private bool _isAsyncLoadSupported;
        private BuildNotifier _notifier;

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();
            _isAsyncLoadSupported = this.IsAsyncPackageSupported();
            if (!_isAsyncLoadSupported)
            {
                _notifier = new BuildNotifier((DTE)GetGlobalService(typeof(DTE)));
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            _notifier?.Dispose();
            base.Dispose(disposing);
        }

        public IVsTask Initialize(IAsyncServiceProvider pServiceProvider, IProfferAsyncService pProfferService, IAsyncProgressCallback pProgressCallback)
        {
            var scheduler = GetService(typeof(SVsTaskSchedulerService)) as IVsTaskSchedulerService;
            return scheduler.Run(VsTaskRunContext.UIThreadIdlePriority, async () =>
            {
                _notifier = new BuildNotifier(await pServiceProvider.GetServiceAsync<DTE>(typeof(DTE)));
            });
        }
    }
}
