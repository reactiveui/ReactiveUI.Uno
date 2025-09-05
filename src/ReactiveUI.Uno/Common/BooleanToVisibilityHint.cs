// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

#if HAS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace ReactiveUI.Uno;

/// <summary>
/// Hints to control how <see cref="bool"/> to <see cref="Visibility"/> conversion behaves.
/// </summary>
[Flags]
public enum BooleanToVisibilityHint
{
    /// <summary>
    /// No special behavior. true => <see cref="Visibility.Visible"/>, false => <see cref="Visibility.Collapsed"/>.
    /// </summary>
    None = 0,

    /// <summary>
    /// Invert the conversion behavior. true => <see cref="Visibility.Collapsed"/>, false => <see cref="Visibility.Visible"/>.
    /// </summary>
    Inverse = 1 << 1,
}
