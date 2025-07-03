// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace ReactiveUI.Uno;

/// <summary>
/// Returns the current orientation of the device on Windows.
/// </summary>
public class PlatformOperations : IPlatformOperations
{
    /// <inheritdoc/>
    public string? GetOrientation()
    {
        try
        {
            return Windows.Graphics.Display.DisplayInformation.GetForCurrentView().CurrentOrientation.ToString();
        }
        catch (Exception)
        {
            return null;
        }
    }
}
