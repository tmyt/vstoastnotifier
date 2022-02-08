using System;
using EnvDTE;
#if DevEnv11
using ToastNotifier.Common;
#else
using ThreadHelperCompat = Microsoft.VisualStudio.Shell.ThreadHelper;
#endif

namespace Vsix.ToastNotifier.Product
{
    public class BuildEvents : IDisposable
    {
        private readonly EnvDTE.BuildEvents _buildEvents;

        public BuildEvents(EnvDTE.BuildEvents buildEvents)
        {
            _buildEvents = buildEvents;
            Register();
        }

        public void Dispose()
        {
            Unregister();
        }

        protected virtual void OnBuildProjConfigBegin(string project, string projectConfig, string platform, string solutionConfig)
        {
        }

        protected virtual void OnBuildProjConfigDone(string project, string projectConfig, string platform, string solutionConfig, bool success)
        {
        }

        protected virtual void OnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
        }

        protected virtual void OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
        }

        private void Register()
        {
            ThreadHelperCompat.ThrowIfNotOnUIThread();
            _buildEvents.OnBuildProjConfigBegin += OnBuildProjConfigBegin;
            _buildEvents.OnBuildProjConfigDone += OnBuildProjConfigDone;
            _buildEvents.OnBuildBegin += OnBuildBegin;
            _buildEvents.OnBuildDone += OnBuildDone;
        }

        private void Unregister()
        {
            ThreadHelperCompat.ThrowIfNotOnUIThread();
            _buildEvents.OnBuildProjConfigBegin -= OnBuildProjConfigBegin;
            _buildEvents.OnBuildProjConfigDone -= OnBuildProjConfigDone;
            _buildEvents.OnBuildBegin -= OnBuildBegin;
            _buildEvents.OnBuildDone -= OnBuildDone;
        }
    }
}
