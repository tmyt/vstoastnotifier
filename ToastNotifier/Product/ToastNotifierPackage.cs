using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Vsix.ToastNotifier.Properties;

namespace Vsix.ToastNotifier.Product
{
    /// <inheritdoc />
    [PackageRegistration(UseManagedResourcesOnly = false)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(Guids.ToastNotifierPackage)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    public sealed class ToastNotifierPackage : Package
    {
        private BuildNotifier _notifier;
        
        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();
            _notifier = new BuildNotifier((DTE)GetGlobalService(typeof(DTE)));
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            _notifier?.Dispose();
            base.Dispose(disposing);
        }
    }
}
