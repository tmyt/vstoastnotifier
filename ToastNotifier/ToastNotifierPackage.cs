using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Windows.UI.Notifications;
using System.Collections;
using System.Collections.Generic;

namespace Company.ToastNotifier
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = false)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidToastNotifierPkgString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    public sealed class ToastNotifierPackage : Package
    {
        private string ApplicationID = "VisualStudio.11.0";

        private int Succeeded = 0;
        private int Failed = 0;
        private DateTime BuildStartedOn;
        private bool BuildStarted;
        private string LastBuiltProject;

        /// <summary>The GetForegroundWindow function returns a handle to the foreground window.</summary>
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public ToastNotifierPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            var dte = (DTE)GetGlobalService(typeof(DTE));
            dte.Events.BuildEvents.OnBuildProjConfigBegin += BuildEvents_OnBuildProjConfigBegin;
            dte.Events.BuildEvents.OnBuildProjConfigDone += BuildEvents_OnBuildProjConfigDone;
            dte.Events.BuildEvents.OnBuildBegin += BuildEvents_OnBuildBegin;
            dte.Events.BuildEvents.OnBuildDone += BuildEvents_OnBuildDone;
        }

        void BuildEvents_OnBuildProjConfigBegin(string Project, string ProjectConfig, string Platform, string SolutionConfig)
        {
            LastBuiltProject = Project;
        }

        void BuildEvents_OnBuildProjConfigDone(string Project, string ProjectConfig, string Platform, string SolutionConfig, bool Success)
        {
            if (Success)
                Succeeded += 1;
            else
                Failed = +1;
        }

        void BuildEvents_OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            Succeeded = 0;
            Failed = 0;
            BuildStartedOn = DateTime.Now;
            BuildStarted = true;
        }

        void BuildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            if (!BuildStarted) return;
            BuildStarted = false;
            var dte = (DTE)GetGlobalService(typeof(DTE));
            if (GetForegroundWindow() == (IntPtr)dte.MainWindow.HWnd) return;
            ShowToast(String.Format("{0} - {1}", Failed == 0 ? Resources.Strings.BuildSucceeded : Resources.Strings.BuildFailed,
                Scope == vsBuildScope.vsBuildScopeSolution ? Path.GetFileNameWithoutExtension(dte.Solution.FullName) : LastBuiltProject),
                String.Format("{0}: {3}, {1}: {4}\n{2}: {5}",
                Resources.Strings.Success, Resources.Strings.Fail, Resources.Strings.TimeElapsed,
                Succeeded, Failed, DateTime.Now - BuildStartedOn));
        }

        void ShowToast(string title, string message)
        {
            // テンプレートを取得（XMLDocument"
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            // textタグを取得（ここに文字列が入る）
            toastXml.GetElementsByTagName("text").First().AppendChild(toastXml.CreateTextNode(title));
            toastXml.GetElementsByTagName("text").Last().AppendChild(toastXml.CreateTextNode(message));

            // Notifierを作成してShowメソッドで通知
            var dte = GetGlobalService(typeof(DTE)) as DTE;
            var notifier = ToastNotificationManager.CreateToastNotifier(EditionToAppUserModelId(dte.Edition));
            notifier.Show(new ToastNotification(toastXml));
        }
        #endregion

        /// <summary>
        /// 各エディションに対するAppUserModelIdを取得する
        /// </summary>
        /// <param name="edition">DTE.Editionの値</param>
        /// <returns>対応するAppUserModelId</returns>
        string EditionToAppUserModelId(string edition)
        {
            switch (edition)
            {
                case "WD Express":
                    return "VWDExpress.11.0";
                case "Desktop Express":
                    return "WDExpress.11.0";
                case "VSWin Express":
                    return "VSWinExpress.11.0";
                case "PD Express":
                    return "VPDExpress.11.0";
            }
            return ApplicationID;
        }

    }
}
