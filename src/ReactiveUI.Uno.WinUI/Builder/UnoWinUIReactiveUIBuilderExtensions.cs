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
public static class UnoWinUIReactiveUIBuilderExtensions
{
    /// <summary>
    /// Gets the blazor main thread scheduler.
    /// </summary>
    /// <value>
    /// The blazor main thread scheduler.
    /// </value>
#if WINDOWS10_0_19041_0
    public static IScheduler UnoMainThreadScheduler { get; } = new WaitForDispatcherScheduler(() => DispatcherQueueScheduler.Current);
#else
    public static IScheduler UnoMainThreadScheduler { get; } = new WaitForDispatcherScheduler(() => CoreDispatcherScheduler.Current);
#endif

    /// <summary>
    /// Configures ReactiveUI for Uno platform with appropriate schedulers.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithUnoWinUI(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder
            .WithUnoWinUIScheduler()
            .WithPlatformModule<Uno.WinUI.Registrations>();
    }

    /// <summary>
    /// Uses WinUI With the Uno scheduler.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithUnoWinUIScheduler(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithMainThreadScheduler(UnoMainThreadScheduler);
    }
}
