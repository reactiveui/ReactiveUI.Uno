// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Uno.SQLiteStudio.Presentation;

namespace ReactiveUI.Uno.SQLiteStudio.Views;

/// <summary>Serves as a base class for main views bound to <see cref="MainViewModel"/>.</summary>
/// <remarks>This class centralizes the generic ReactiveUI base type required by XAML.</remarks>
public class MainViewBase : ReactiveUserControl<MainViewModel>;
