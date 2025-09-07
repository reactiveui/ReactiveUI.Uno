// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno;

/// <summary>
/// Returns the current orientation of the device on Windows.
/// </summary>
public class PlatformOperations : IPlatformOperations
{
    /// <inheritdoc/>
    public string? GetOrientation()
    {
#if WINDOWS
        // WinAppSDK (WinUI 3) does not have a CoreWindow. Avoid calling GetForCurrentView when no CoreWindow is present.
        try
        {
            var hasCoreWindow = false;
            try
            {
                // If a CoreWindow is not attached to this thread, this API may throw.
                var cw = Windows.UI.Core.CoreWindow.GetForCurrentThread();
                hasCoreWindow = cw is not null;
            }
            catch
            {
                hasCoreWindow = false;
            }

            if (!hasCoreWindow)
            {
                return null;
            }

            return Windows.Graphics.Display.DisplayInformation.GetForCurrentView().CurrentOrientation.ToString();
        }
        catch
        {
            return null;
        }
#else
        try
        {
            return Windows.Graphics.Display.DisplayInformation.GetForCurrentView().CurrentOrientation.ToString();
        }
        catch
        {
            return null;
        }
#endif
    }
}
