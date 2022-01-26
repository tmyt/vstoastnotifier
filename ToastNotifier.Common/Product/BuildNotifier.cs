using System;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Vsix.ToastNotifier.Extensions;
#if DevEnv11
using ToastNotifier.Common;
#else
using ThreadHelperCompat = Microsoft.VisualStudio.Shell.ThreadHelper;
#endif

namespace Vsix.ToastNotifier.Product
{
    public class BuildNotifier : BuildEvents
    {
        private int _succeeded;
        private int _failed;
        private DateTime _buildStartedOn;
        private bool _buildIsProgress;
        private string _lastBuiltProject;

        public BuildNotifier(DTE dte) : base(dte)
        {
        }

        protected override void OnBuildProjConfigBegin(string project, string projectConfig, string platform, string solutionConfig)
        {
            _lastBuiltProject = project;
        }

        protected override void OnBuildProjConfigDone(string project, string projectConfig, string platform, string solutionConfig, bool success)
        {
            if (success)
                _succeeded += 1;
            else
                _failed += 1;
        }

        protected override void OnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
            _succeeded = 0;
            _failed = 0;
            _buildStartedOn = DateTime.Now;
            _buildIsProgress = true;
        }

        protected override void OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            if (!_buildIsProgress) return;
            _buildIsProgress = false;
            new ToastBuilder
            {
                TargetName = GetTargetName(scope),
                Succeeded = _succeeded,
                Failed = _failed,
                Elapsed = DateTime.Now - _buildStartedOn,
            }.Build().Show();
        }

        private string GetTargetName(vsBuildScope scope)
        {
            ThreadHelperCompat.ThrowIfNotOnUIThread();
            var dte = (DTE)Package.GetGlobalService(typeof(DTE));
            return scope == vsBuildScope.vsBuildScopeSolution
                ? Path.GetFileNameWithoutExtension(dte.Solution.FullName)
                : _lastBuiltProject;
        }
    }
}
