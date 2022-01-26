using System;
using Windows.UI.Notifications;

namespace Vsix.ToastNotifier.Product
{
    public class ToastBuilder
    {
        public string TargetName { get; set; }
        public int Succeeded { get; set; }
        public int Failed { get; set; }
        public TimeSpan Elapsed { get; set; }
        public string State => Failed == 0 ? Resources.Strings.BuildSucceeded : Resources.Strings.BuildFailed;

        public ToastNotification Build()
        {
            // Get toast template（XMLDocument)
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            // Set text contents
            var tags = toastXml.GetElementsByTagName("text");
            tags[0].InnerText = $"{State} - {TargetName}";
            tags[1].InnerText = $"{SucceededLabel()}, {FailedLabel()}\n{ElapsedLabel()}";
            return new ToastNotification(toastXml);
        }

        private string SucceededLabel()
        {
            return $"{Resources.Strings.Success}: {Succeeded}";
        }

        private string FailedLabel()
        {
            return $"{Resources.Strings.Fail}: {Failed}";
        }

        private string ElapsedLabel()
        {
            return $"{Resources.Strings.TimeElapsed}: {Elapsed}";
        }
    }
}
