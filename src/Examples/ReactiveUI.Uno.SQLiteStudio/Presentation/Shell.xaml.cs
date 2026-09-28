// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;

namespace ReactiveUI.Uno.SQLiteStudio.Presentation;

/// <summary>
/// Represents the main user interface container for the application, providing the root visual structure and navigation
/// context.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Shell: {ToString(),nq}")]
public sealed partial class Shell : UserControl
{
    /// <summary>Initializes a new instance of the <see cref="Shell"/> class.</summary>
    public Shell() => InitializeComponent();
}
