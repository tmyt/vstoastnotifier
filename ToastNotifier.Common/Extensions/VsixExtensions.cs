using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Vsix.ToastNotifier.Extensions
{
    static class VsixExtensions
    {
        public static IVsTask CreateTask(this IVsTaskSchedulerService service, VsTaskRunContext context, Action action)
        {
            return service.CreateTask(context, VsTaskLibraryHelper.CreateTaskBody(action));
        }

        public static IVsTask Run(this IVsTaskSchedulerService service, VsTaskRunContext context, Action action)
        {
            var task = service.CreateTask(context, action);
            task.Start();
            return task;
        }
    }
}
