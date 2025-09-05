// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;

namespace ReactiveUI.Builder;

/// <summary>
/// Uno-specific extensions for the ReactiveUI builder.
/// </summary>
public static class UnoReactiveUIBuilderExtensions
{
    /// <summary>
    /// Gets the Uno main thread scheduler. Work scheduled on this scheduler is marshaled to the UI thread.
    /// </summary>
    public static IScheduler UnoMainThreadScheduler { get; } = new WaitForDispatcherScheduler(() => CoreDispatcherScheduler.Current);

    /// <summary>
    /// Configures ReactiveUI for Uno platform with appropriate schedulers and default services.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is null.</exception>
    public static IReactiveUIBuilder WithUno(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder
            .WithUnoScheduler()
            .WithPlatformModule<Uno.Registrations>();
    }

    /// <summary>
    /// Registers the Uno main-thread scheduler with the ReactiveUI configuration.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is null.</exception>
    public static IReactiveUIBuilder WithUnoScheduler(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithMainThreadScheduler(UnoMainThreadScheduler);
    }
}
