// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using ReactiveUI.Builder;
using Splat;
using Splat.Builder;

namespace ReactiveUI.Uno;

/// <summary>
/// Registers platform-specific services, type converters, and binding hooks required for ReactiveUI to operate on the
/// Uno platform.
/// </summary>
/// <remarks>This class is typically used internally by the ReactiveUI Uno integration to ensure that all
/// necessary services and type converters are available at application startup. It is not intended to be used directly
/// by application code.</remarks>
public class Registrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(IRegistrar registrar)
    {
        ArgumentNullException.ThrowIfNull(registrar);

        registrar.RegisterConstant<IPlatformOperations>(() => new PlatformOperations());
        registrar.RegisterConstant<IActivationForViewFetcher>(() => new ActivationForViewFetcher());
        registrar.RegisterConstant<ICreatesObservableForProperty>(() => new DependencyObjectObservableForProperty());

        registrar.RegisterConstant<IBindingTypeConverter>(() => new StringConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(() => new ByteToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(() => new NullableByteToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(() => new ShortToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(() => new NullableShortToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(() => new IntegerToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(() => new NullableIntegerToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(() => new LongToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(() => new NullableLongToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(() => new SingleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(() => new NullableSingleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(() => new DoubleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(() => new NullableDoubleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(() => new DecimalToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(() => new NullableDecimalToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(() => new BooleanToVisibilityTypeConverter());

        registrar.RegisterConstant<IPropertyBindingHook>(() => new AutoDataTemplateBindingHook());
        registrar.RegisterConstant<ISuspensionDriver>(() => new WinRTAppDataDriver());

        if (!ModeDetector.InUnitTestRunner() && !AppBuilder.UsingBuilder)
        {
#if WINDOWS
            RxSchedulers.MainThreadScheduler = UnoReactiveUIBuilderExtensions.UnoWinUIMainThreadScheduler;
#else
            RxSchedulers.MainThreadScheduler = UnoReactiveUIBuilderExtensions.UnoMainThreadScheduler;
#endif

#if __WASM__ || BROWSERWASM
            // WebAssembly doesn't support multithreading, use WasmScheduler instead of TaskPoolScheduler
            RxSchedulers.TaskpoolScheduler = WasmScheduler.Default;
#else
            RxSchedulers.TaskpoolScheduler = TaskPoolScheduler.Default;
#endif
        }

        // Disables ViewCommand binding messages on Uno platform
        RxSchedulers.SuppressViewCommandBindingMessage = true;
    }
}
