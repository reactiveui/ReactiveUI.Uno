// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Uno.SQLiteStudio.Presentation;

namespace ReactiveUI.Uno.SQLiteStudio.Views;

/// <summary>Serves as a base class for main views bound to <see cref="MainViewModel"/>.</summary>
/// <remarks>This class centralizes the generic ReactiveUI base type required by XAML.</remarks>
public class MainViewBase : ReactiveUserControl<MainViewModel>;
