// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno;

/// <summary>
/// Represents the application's main bootstrapper, providing routing and navigation state for the user interface.
/// </summary>
/// <remarks>AppBootstrapper serves as the entry point for initializing and managing navigation within the
/// application. It implements the IScreen interface, which is used by ReactiveUI for view model routing. This class is
/// typically used to configure the application's initial navigation state and to provide a central location for routing
/// logic.</remarks>
public sealed partial class AppBootstrapper() : ReactiveObject, IScreen
{
    /// <summary>
    /// Gets the Router associated with this Screen.
    /// </summary>
    public RoutingState Router { get; } = new RoutingState();
}
