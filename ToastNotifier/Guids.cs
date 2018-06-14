// Guids.cs
// MUST match guids.h

using System;

namespace Vsix.ToastNotifier
{
    static class GuidList
    {
        public const string guidToastNotifierPkgString = "8206e3d6-b530-44a4-8490-ea60aefaa24a";
        public const string guidToastNotifierCmdSetString = "6c2306af-040e-4797-882f-c3e75f0d20e1";

        public static readonly Guid guidToastNotifierCmdSet = new Guid(guidToastNotifierCmdSetString);
    };
}