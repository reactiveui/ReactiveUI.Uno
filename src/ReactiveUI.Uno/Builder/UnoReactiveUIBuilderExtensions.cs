// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if __WASM__ || BROWSERWASM
#if REACTIVE_SHIM
using ReactiveUI.Primitives.Blazor.Reactive.Concurrency;
#else
using ReactiveUI.Primitives.Blazor.Concurrency;
#endif
using BlazorDispatcher = Microsoft.AspNetCore.Components.Dispatcher;
#endif
#if REACTIVE_SHIM
using ReactiveUI.Uno.Reactive;
using UnoRegistrations = ReactiveUI.Uno.Reactive.Registrations;
#else
using ReactiveUI.Primitives.Concurrency;
using ReactiveUI.Uno;
using UnoRegistrations = ReactiveUI.Uno.Registrations;
#endif

namespace ReactiveUI.Builder;

/// <summary>Uno-specific extensions for the ReactiveUI builder.</summary>
public static class UnoReactiveUIBuilderExtensions
{
#if WINDOWS
    /// <summary>Gets the Uno WinUI main thread scheduler.</summary>
    public static ISequencer UnoWinUIMainThreadScheduler { get; } = new WaitForDispatcherScheduler(() => UnoWinUIDispatcherScheduler.Current);
#else
    /// <summary>Gets the Uno main thread scheduler.</summary>
    public static ISequencer UnoMainThreadScheduler { get; } = new WaitForDispatcherScheduler(() => UnoDispatcherScheduler.Current);
#endif

    /// <summary>Provides extension members for ReactiveUI builders.</summary>
    /// <param name="builder">The builder to configure.</param>
    extension(IReactiveUIBuilder builder)
    {
#if WINDOWS
        /// <summary>Uses WinUI with the Uno scheduler.</summary>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithUnoScheduler()
        {
            ArgumentNullException.ThrowIfNull(builder);

            return builder.WithMainThreadScheduler(UnoWinUIMainThreadScheduler);
        }
#else
        /// <summary>Registers the Uno main-thread scheduler with the ReactiveUI configuration.</summary>
        /// <returns>The builder instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the builder is null.</exception>
        public IReactiveUIBuilder WithUnoScheduler()
        {
            ArgumentNullException.ThrowIfNull(builder);

            return builder.WithMainThreadScheduler(UnoMainThreadScheduler);
        }
#endif

        /// <summary>Gets the scheduler that schedules work on the Uno Platform main thread.</summary>
        /// <remarks>Use this scheduler to ensure that UI-related work is executed on the Uno main thread. This is
        /// particularly important for operations that interact with UI elements, as they must be performed on the main
        /// thread to avoid threading issues.</remarks>
        /// <param name="startupWindow">The startup window.</param>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithUno(Window startupWindow)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(startupWindow);

            return AddUnoDictionary(
                builder
                    .WithRegistration(mutable => new UnoRegistrations().Register(new DependencyResolverRegistrar(mutable)))
                    .WithMainThreadScheduler(GetUnoMainThreadScheduler(startupWindow))
                    .WithTaskPoolScheduler(GetUnoTaskPoolScheduler()),
                startupWindow);
        }

        /// <summary>Registers a default IScreen implementation.</summary>
        /// <returns>The builder instance for chaining.</returns>
        public IReactiveUIBuilder WithDefaultIScreen()
        {
            ArgumentNullException.ThrowIfNull(builder);

            return builder.WithRegistration(mutable => mutable.RegisterConstant<IScreen>(new AppBootstrapper()));
        }
    }

    /// <summary>Gets the scheduler used for background work on the current platform.</summary>
    /// <returns>The scheduler used for background work.</returns>
    internal static ISequencer GetUnoTaskPoolScheduler() =>
#if __WASM__ || BROWSERWASM
        new BlazorRendererSequencer(BlazorDispatcher.CreateDefault());
#else
        TaskPoolSequencer.Default;
#endif

#if WINDOWS
    /// <summary>Creates the Uno main-thread scheduler from the startup window dispatcher.</summary>
    /// <param name="startupWindow">The startup window.</param>
    /// <returns>The scheduler used to marshal work to the startup window dispatcher.</returns>
    internal static ISequencer GetUnoMainThreadScheduler(Window startupWindow)
    {
        ArgumentNullException.ThrowIfNull(startupWindow);

        return new UnoWinUIDispatcherScheduler(startupWindow.DispatcherQueue);
    }
#else
    /// <summary>Creates the Uno main-thread scheduler from the startup window dispatcher.</summary>
    /// <param name="startupWindow">The startup window.</param>
    /// <returns>The scheduler used to marshal work to the startup window dispatcher.</returns>
    internal static ISequencer GetUnoMainThreadScheduler(Window startupWindow)
    {
        ArgumentNullException.ThrowIfNull(startupWindow);

        return new UnoDispatcherScheduler(startupWindow.Dispatcher);
    }
#endif

#if WINDOWS
    /// <summary>Gets the current Uno main-thread scheduler.</summary>
    /// <returns>The Uno main-thread scheduler.</returns>
    internal static ISequencer GetUnoMainThreadRxScheduler() => new DeferredScheduler(() => UnoWinUIDispatcherScheduler.Current);
#else
    /// <summary>Gets the current Uno main-thread scheduler.</summary>
    /// <returns>The Uno main-thread scheduler.</returns>
    internal static ISequencer GetUnoMainThreadRxScheduler() => new DeferredScheduler(() => UnoDispatcherScheduler.Current);
#endif

    /// <summary>Adds the Uno resource dictionary to the application when the startup window is activated.</summary>
    /// <remarks>This method ensures that the Uno.ReactiveUIUnoDictionary is added to the application's merged
    /// resource dictionaries after the startup window is activated. This setup is performed only once per application
    /// startup.</remarks>
    /// <param name="builder">The reactive UI builder to configure. Cannot be null.</param>
    /// <param name="startupWindow">The window whose activation triggers the addition of the Uno dictionary. Cannot be null.</param>
    /// <returns>The same instance of <paramref name="builder"/> for method chaining.</returns>
    private static IReactiveUIBuilder AddUnoDictionary(IReactiveUIBuilder builder, Window startupWindow)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(startupWindow);
        startupWindow.Activated += SetupDictionary;
        return builder;

        static void SetupDictionary(object obj, WindowActivatedEventArgs args)
        {
            // Ensure the dictionary is added after build is complete
            if (!Application.Current.Resources.MergedDictionaries.Contains(new ReactiveUIUnoDictionary()))
            {
                Application.Current.Resources.MergedDictionaries.Add(new ReactiveUIUnoDictionary());
            }

            Window.Current?.Activated -= SetupDictionary;
        }
    }
}
