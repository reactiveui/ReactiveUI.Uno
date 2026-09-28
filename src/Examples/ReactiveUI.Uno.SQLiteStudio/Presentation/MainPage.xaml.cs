// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;

namespace ReactiveUI.Uno.SQLiteStudio.Presentation;

/// <summary>Represents the main page of the application.</summary>
/// <remarks>This page serves as the entry point for the application's user interface. It is typically loaded when
/// the application starts and may contain navigation controls or primary content for the user.</remarks>
[System.Diagnostics.DebuggerDisplay("MainPage: {ToString(),nq}")]
public sealed partial class MainPage : Page
{
    /// <summary>Initializes a new instance of the <see cref="MainPage"/> class.</summary>
    public MainPage() => InitializeComponent();
}
