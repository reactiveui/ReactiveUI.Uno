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
    public static IScheduler UnoWinUIMainThreadScheduler { get; } = new WaitForDispatcherScheduler(() => UnoWinUIDispatcherScheduler.Current);

    /// <summary>
    /// Uses WinUI with the Uno scheduler.
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
#if __WASM__ || BROWSERWASM
            // WebAssembly doesn't support multithreading, use WasmScheduler instead of TaskPoolScheduler
            .WithTaskPoolScheduler(WasmScheduler.Default)
#else
            .WithTaskPoolScheduler(TaskPoolScheduler.Default)
#endif
            .WithPlatformModule<Uno.Registrations>()
            .WithUnoDictionary();
    }

    /// <summary>
    /// Registers a default IScreen implementation.
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
    /// Builds the application and returns the ReactiveUI instance wrapper.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if building the app instance fails.</exception>
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
    /// Resolves a single instance and passes it to the action.
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

        action?.Invoke(reactiveUIInstance.Current.GetService<T>());
        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves two instances and passes them to the action.
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
    /// Resolves three instances and passes them to the action.
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
    /// Resolves four instances and passes them to the action.
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
    /// Resolves five instances and passes them to the action.
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

    /// <summary>
    /// Resolves six instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fith type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2?>(),
                   current.GetService<T3?>(),
                   current.GetService<T4?>(),
                   current.GetService<T5?>(),
                   current.GetService<T6?>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves seven instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fith type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2?>(),
                   current.GetService<T3?>(),
                   current.GetService<T4?>(),
                   current.GetService<T5?>(),
                   current.GetService<T6?>(),
                   current.GetService<T7?>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves eight instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fith type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2?>(),
                   current.GetService<T3?>(),
                   current.GetService<T4?>(),
                   current.GetService<T5?>(),
                   current.GetService<T6?>(),
                   current.GetService<T7?>(),
                   current.GetService<T8?>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves nine instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fith type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2?>(),
                   current.GetService<T3?>(),
                   current.GetService<T4?>(),
                   current.GetService<T5?>(),
                   current.GetService<T6?>(),
                   current.GetService<T7?>(),
                   current.GetService<T8?>(),
                   current.GetService<T9?>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves ten instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fith type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <typeparam name="T10">The tenth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2?>(),
                   current.GetService<T3?>(),
                   current.GetService<T4?>(),
                   current.GetService<T5?>(),
                   current.GetService<T6?>(),
                   current.GetService<T7?>(),
                   current.GetService<T8?>(),
                   current.GetService<T9?>(),
                   current.GetService<T10?>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves eleven instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fith type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <typeparam name="T10">The tenth type to resolve.</typeparam>
    /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2?>(),
                   current.GetService<T3?>(),
                   current.GetService<T4?>(),
                   current.GetService<T5?>(),
                   current.GetService<T6?>(),
                   current.GetService<T7?>(),
                   current.GetService<T8?>(),
                   current.GetService<T9?>(),
                   current.GetService<T10?>(),
                   current.GetService<T11?>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves twelve instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fith type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <typeparam name="T10">The tenth type to resolve.</typeparam>
    /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
    /// <typeparam name="T12">The twelfth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2?>(),
                   current.GetService<T3?>(),
                   current.GetService<T4?>(),
                   current.GetService<T5?>(),
                   current.GetService<T6?>(),
                   current.GetService<T7?>(),
                   current.GetService<T8?>(),
                   current.GetService<T9?>(),
                   current.GetService<T10?>(),
                   current.GetService<T11?>(),
                   current.GetService<T12?>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves thirteen instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fith type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <typeparam name="T10">The tenth type to resolve.</typeparam>
    /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
    /// <typeparam name="T12">The twelfth type to resolve.</typeparam>
    /// <typeparam name="T13">The thirteenth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2?>(),
                   current.GetService<T3?>(),
                   current.GetService<T4?>(),
                   current.GetService<T5?>(),
                   current.GetService<T6?>(),
                   current.GetService<T7?>(),
                   current.GetService<T8?>(),
                   current.GetService<T9?>(),
                   current.GetService<T10?>(),
                   current.GetService<T11?>(),
                   current.GetService<T12?>(),
                   current.GetService<T13?>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves fourteen instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fith type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <typeparam name="T10">The tenth type to resolve.</typeparam>
    /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
    /// <typeparam name="T12">The twelfth type to resolve.</typeparam>
    /// <typeparam name="T13">The thirteenth type to resolve.</typeparam>
    /// <typeparam name="T14">The fourteenth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2?>(),
                   current.GetService<T3?>(),
                   current.GetService<T4?>(),
                   current.GetService<T5?>(),
                   current.GetService<T6?>(),
                   current.GetService<T7?>(),
                   current.GetService<T8?>(),
                   current.GetService<T9?>(),
                   current.GetService<T10?>(),
                   current.GetService<T11?>(),
                   current.GetService<T12?>(),
                   current.GetService<T13?>(),
                   current.GetService<T14?>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves fifteen instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fith type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <typeparam name="T10">The tenth type to resolve.</typeparam>
    /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
    /// <typeparam name="T12">The twelfth type to resolve.</typeparam>
    /// <typeparam name="T13">The thirteenth type to resolve.</typeparam>
    /// <typeparam name="T14">The fourteenth type to resolve.</typeparam>
    /// <typeparam name="T15">The fithteenth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2?>(),
                   current.GetService<T3?>(),
                   current.GetService<T4?>(),
                   current.GetService<T5?>(),
                   current.GetService<T6?>(),
                   current.GetService<T7?>(),
                   current.GetService<T8?>(),
                   current.GetService<T9?>(),
                   current.GetService<T10?>(),
                   current.GetService<T11?>(),
                   current.GetService<T12?>(),
                   current.GetService<T13?>(),
                   current.GetService<T14?>(),
                   current.GetService<T15?>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves sixteen instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fith type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <typeparam name="T10">The tenth type to resolve.</typeparam>
    /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
    /// <typeparam name="T12">The twelfth type to resolve.</typeparam>
    /// <typeparam name="T13">The thirteenth type to resolve.</typeparam>
    /// <typeparam name="T14">The fourteenth type to resolve.</typeparam>
    /// <typeparam name="T15">The fithteenth type to resolve.</typeparam>
    /// <typeparam name="T16">The sixteenth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="System.ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?, T16?> action)
    {
        if (reactiveUIInstance is null || reactiveUIInstance.Current is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                current.GetService<T1>(),
                current.GetService<T2?>(),
                current.GetService<T3?>(),
                current.GetService<T4?>(),
                current.GetService<T5?>(),
                current.GetService<T6?>(),
                current.GetService<T7?>(),
                current.GetService<T8?>(),
                current.GetService<T9?>(),
                current.GetService<T10?>(),
                current.GetService<T11?>(),
                current.GetService<T12?>(),
                current.GetService<T13?>(),
                current.GetService<T14?>(),
                current.GetService<T15?>(),
                current.GetService<T16?>());
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
