// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;

namespace ReactiveUI.Uno;

/// <summary>
/// UWP platform registrations.
/// </summary>
/// <seealso cref="ReactiveUI.IWantsToRegisterStuff" />
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

        RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
#if WINDOWS10_0_19041_0
        RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => DispatcherQueueScheduler.Current);
#else
        RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => CoreDispatcherScheduler.Current);
#endif
    }
}
