using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Vsix.ToastNotifier.Extensions;
using Vsix.ToastNotifier.Properties;

#if DevEnv11
using Microsoft.VisualStudio.AsyncPackageHelpers;
using AsyncPackage = Microsoft.VisualStudio.Shell.Package;
using ProvideAutoLoadCompatAttribute = Microsoft.VisualStudio.AsyncPackageHelpers.ProvideAutoLoadAttribute;
#else
using System.ComponentModel.Composition;
using AsyncPackageRegistrationAttribute = Microsoft.VisualStudio.Shell.PackageRegistrationAttribute;
using ProvideAutoLoadCompatAttribute = Microsoft.VisualStudio.Shell.ProvideAutoLoadAttribute;
#endif

namespace Vsix.ToastNotifier.Product
{
    /// <inheritdoc />
    [AsyncPackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(Guids.ToastNotifierPackage)]
    [ProvideAutoLoadCompat(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class ToastNotifierPackage : AsyncPackage, IAsyncLoadablePackageInitialize
    {
        private BuildNotifier _notifier;
        public EnvDTE.BuildEvents BuildEvents { get; set; }


#if DevEnv11
        private bool _isAsyncLoadSupported;
#else
        [Import]
        internal SVsServiceProvider ServiceProvider = null;
#endif

#if DevEnv11
        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();
            _isAsyncLoadSupported = this.IsAsyncPackageSupported();
            if (!_isAsyncLoadSupported)
            {
                var dte = (DTE)GetGlobalService(typeof(DTE));
                BuildEvents = dte.Events.BuildEvents;
                _notifier = new BuildNotifier(BuildEvents);
            }
        }
        
        public IVsTask Initialize(IAsyncServiceProvider pServiceProvider, IProfferAsyncService pProfferService, IAsyncProgressCallback pProgressCallback)
        {
            var scheduler = GetService(typeof(SVsTaskSchedulerService)) as IVsTaskSchedulerService;
            return scheduler.Run(VsTaskRunContext.UIThreadIdlePriority, async () =>
            {
                var dte = await pServiceProvider.GetServiceAsync<DTE>(typeof(DTE));
                BuildEvents = dte.Events.BuildEvents;
                _notifier = new BuildNotifier(BuildEvents);
            });
        }
#else
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            var dte = (DTE)await GetServiceAsync(typeof(DTE));
            BuildEvents = dte.Events.BuildEvents;
            _notifier = new BuildNotifier(BuildEvents);
        }
#endif

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            _notifier?.Dispose();
            base.Dispose(disposing);
        }
    }
}
