// This File came from github.com/Microsoft/VSSDK-Extensibility-Samples
/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015 Microsoft
 * Copyright (c) 2019 Yutaka TSUMORI
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using Microsoft.VisualStudio.Shell.Interop;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.AsyncPackageHelpers
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Helper method to use async/await with IAsyncServiceProvider implementation
        /// </summary>
        /// <param name="asyncServiceProvider">IAsyncServciceProvider instance</param>
        /// <param name="serviceType">Type of the Visual Studio service requested</param>
        /// <returns>Service object as type of T</returns>
        public static async Task<T> GetServiceAsync<T>(this IAsyncServiceProvider asyncServiceProvider, Type serviceType) where T : class
        {
            // We have to make sure we are on main UI thread before trying to cast as underlying implementation
            // can be an STA COM object and doing a cast would require calling QueryInterface/AddRef marshaling 
            // to main thread via COM.
            return await System.Threading.Tasks.Task.Run(async () =>
            {
                Guid serviceTypeGuid = serviceType.GUID;
                return await asyncServiceProvider.QueryServiceAsync(ref serviceTypeGuid);
            }).ConfigureAwait(true) as T;
        }

        /// <summary>
        /// Gets if async package is supported in the current instance of Visual Studio
        /// </summary>
        /// <param name="serviceProvider">an IServiceProvider instance, usually a Package instance</param>
        /// <returns>true if async packages are supported</returns>
        public static bool IsAsyncPackageSupported(this IServiceProvider serviceProvider)
        {
            IAsyncServiceProvider asyncServiceProvider = serviceProvider.GetService(typeof(SAsyncServiceProvider)) as IAsyncServiceProvider;
            return asyncServiceProvider != null;
        }
    }
}