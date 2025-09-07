// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using ReactiveUI.Uno;
using Splat;

namespace ReactiveUI.Builder;

/// <summary>
/// Uno-specific extensions for the ReactiveUI builder.
/// </summary>
public static class UnoReactiveUIBuilderExtensions
{
#if WINDOWS
    /// <summary>
    /// Gets the Uno WinUI main thread scheduler.
    /// </summary>
    /// <value>
    /// The Uno WinUI main thread scheduler.
    /// </value>
    public static IScheduler UnoWinUIMainThreadScheduler { get; } = new WaitForDispatcherScheduler(() => UnoWinUIDispatcherScheduler.Current);

    /// <summary>
    /// Uses WinUI With the Uno scheduler.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithUnoScheduler(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithMainThreadScheduler(UnoWinUIMainThreadScheduler);
    }

#else
    /// <summary>
    /// Gets the Uno main thread scheduler.
    /// </summary>
    /// <value>
    /// The Uno main thread scheduler.
    /// </value>
    public static IScheduler UnoMainThreadScheduler { get; } = new WaitForDispatcherScheduler(() => UnoDispatcherScheduler.Current);

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
#endif

    /// <summary>
    /// Configures ReactiveUI for Uno platform with appropriate schedulers.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithUno(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder
            .WithUnoScheduler()
            .WithTaskPoolScheduler(TaskPoolScheduler.Default)
            .WithPlatformModule<Uno.Registrations>()
            .WithUnoDictionary();
    }

    /// <summary>
    /// Withes the default i screen.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder instance for chaining.</returns>
    [RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
    public static IReactiveUIBuilder WithDefaultIScreen(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithRegistration(mutable => mutable.RegisterConstant<IScreen>(new AppBootstrapper()));
    }

    /// <summary>
    /// Builds the application.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">builder.</exception>
    /// <exception cref="System.InvalidOperationException">Failed to create AppLocator instance.</exception>
    public static IReactiveUIInstance BuildApp(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (builder.Build() is not IReactiveUIInstance appInstance || appInstance.Current is null)
        {
            throw new InvalidOperationException("Failed to create AppLocator instance");
        }

        return appInstance;
    }

    /// <summary>
    /// Withes the instance.
    /// </summary>
    /// <typeparam name="T">The type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T>(this IReactiveUIInstance reactiveUIInstance, Action<T?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            action(reactiveUIInstance.Current.GetService<T>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Withes the instance.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(current.GetService<T1>(), current.GetService<T2?>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Withes the instance.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(current.GetService<T1>(), current.GetService<T2?>(), current.GetService<T3?>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Withes the instance.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(current.GetService<T1>(), current.GetService<T2?>(), current.GetService<T3?>(), current.GetService<T4?>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Withes the instance.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fith type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(current.GetService<T1>(), current.GetService<T2?>(), current.GetService<T3?>(), current.GetService<T4?>(), current.GetService<T5?>());
        }

        return reactiveUIInstance;
    }

    private static IReactiveUIBuilder WithUnoDictionary(this IReactiveUIBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        Application.Current.Resources.MergedDictionaries.Add(new Uno.ReactiveUIUnoDictionary());
        return builder;
    }
}
