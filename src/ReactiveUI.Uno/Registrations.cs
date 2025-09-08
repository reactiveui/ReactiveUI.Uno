// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using Splat;
using Splat.Builder;

namespace ReactiveUI.Uno;

/// <summary>
/// Uno platform registrations.
/// </summary>
/// <seealso cref="IWantsToRegisterStuff" />
public class Registrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(Action<Func<object>, Type> registerFunction)
    {
        ArgumentNullException.ThrowIfNull(registerFunction);

        registerFunction(() => new PlatformOperations(), typeof(IPlatformOperations));
        registerFunction(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
        registerFunction(() => new DependencyObjectObservableForProperty(), typeof(ICreatesObservableForProperty));

        registerFunction(() => new StringConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new ByteToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new NullableByteToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new ShortToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new NullableShortToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new IntegerToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new NullableIntegerToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new LongToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new NullableLongToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new SingleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new NullableSingleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new DoubleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new NullableDoubleToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new DecimalToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new NullableDecimalToStringTypeConverter(), typeof(IBindingTypeConverter));
        registerFunction(() => new BooleanToVisibilityTypeConverter(), typeof(IBindingTypeConverter));

        registerFunction(() => new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
        registerFunction(() => new WinRTAppDataDriver(), typeof(ISuspensionDriver));

        if (!ModeDetector.InUnitTestRunner() && !AppBuilder.UsingBuilder)
        {
#if WINDOWS
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => UnoWinUIDispatcherScheduler.Current);
#else
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => UnoDispatcherScheduler.Current);
#endif
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
        }

        // Disables ViewCommand binding messages on Uno platform
        RxApp.SuppressViewCommandBindingMessage = true;
    }
}
