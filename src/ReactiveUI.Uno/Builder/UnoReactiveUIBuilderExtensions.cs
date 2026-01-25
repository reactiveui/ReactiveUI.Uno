// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using ReactiveUI.Uno;

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
        ArgumentNullException.ThrowIfNull(builder);

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
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithMainThreadScheduler(UnoMainThreadScheduler);
    }
#endif

    /// <summary>
    /// Gets the scheduler that schedules work on the Uno Platform main thread.
    /// </summary>
    /// <remarks>Use this scheduler to ensure that UI-related work is executed on the Uno main thread. This is
    /// particularly important for operations that interact with UI elements, as they must be performed on the main
    /// thread to avoid threading issues.</remarks>
    /// <param name="builder">The builder instance.</param>
    /// <param name="startupWindow">The startup window.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithUno(this IReactiveUIBuilder builder, Window startupWindow)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(startupWindow);

        return builder
            .WithPlatformModule<Uno.Registrations>()
            .WithUnoScheduler()
#if __WASM__ || BROWSERWASM
            // WebAssembly doesn't support multithreading, use WasmScheduler instead of TaskPoolScheduler
            .WithTaskPoolScheduler(WasmScheduler.Default)
#else
            .WithTaskPoolScheduler(TaskPoolScheduler.Default)
#endif
            .WithUnoDictionary(startupWindow);
    }

    /// <summary>
    /// Registers a default IScreen implementation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IReactiveUIBuilder WithDefaultIScreen(this IReactiveUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithRegistration(mutable => mutable.RegisterConstant<IScreen>(new AppBootstrapper()));
    }

    /// <summary>
    /// Configures the specified reactive UI builder to include the Uno dictionary in the application's resource
    /// dictionaries when the startup window is activated.
    /// </summary>
    /// <remarks>This method ensures that the Uno.ReactiveUIUnoDictionary is added to the application's merged
    /// resource dictionaries after the startup window is activated. This setup is performed only once per application
    /// startup.</remarks>
    /// <param name="builder">The reactive UI builder to configure. Cannot be null.</param>
    /// <param name="startupWindow">The window whose activation triggers the addition of the Uno dictionary. Cannot be null.</param>
    /// <returns>The same instance of <paramref name="builder"/> for method chaining.</returns>
    private static IReactiveUIBuilder WithUnoDictionary(this IReactiveUIBuilder builder, Window startupWindow)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(startupWindow);
        startupWindow.Activated += SetupDictionary;
        return builder;

        static void SetupDictionary(object obj, WindowActivatedEventArgs args)
        {
            // Ensure the dictionary is added after build is complete
            if (!Application.Current.Resources.MergedDictionaries.Contains(new Uno.ReactiveUIUnoDictionary()))
            {
                Application.Current.Resources.MergedDictionaries.Add(new Uno.ReactiveUIUnoDictionary());
            }

            Window.Current?.Activated -= SetupDictionary;
        }
    }
}
