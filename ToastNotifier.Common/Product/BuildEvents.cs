﻿using System;
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
        private readonly DTE _dte;

        public BuildEvents(DTE dte)
        {
            _dte = dte;
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
            _dte.Events.BuildEvents.OnBuildProjConfigBegin += OnBuildProjConfigBegin;
            _dte.Events.BuildEvents.OnBuildProjConfigDone += OnBuildProjConfigDone;
            _dte.Events.BuildEvents.OnBuildBegin += OnBuildBegin;
            _dte.Events.BuildEvents.OnBuildDone += OnBuildDone;
        }

        private void Unregister()
        {
            ThreadHelperCompat.ThrowIfNotOnUIThread();
            _dte.Events.BuildEvents.OnBuildProjConfigBegin -= OnBuildProjConfigBegin;
            _dte.Events.BuildEvents.OnBuildProjConfigDone -= OnBuildProjConfigDone;
            _dte.Events.BuildEvents.OnBuildBegin -= OnBuildBegin;
            _dte.Events.BuildEvents.OnBuildDone -= OnBuildDone;
        }
    }
}
