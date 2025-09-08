// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Uno;

/// <summary>
/// A default App Bootstrapper.
/// </summary>
/// <seealso cref="ReactiveObject" />
/// <seealso cref="IScreen" />
/// <remarks>
/// Initializes a new instance of the <see cref="AppBootstrapper"/> class.
/// </remarks>
[RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
public sealed partial class AppBootstrapper() : ReactiveObject, IScreen
{
    /// <summary>
    /// Gets the Router associated with this Screen.
    /// </summary>
    public RoutingState Router { get; } = new RoutingState();
}
