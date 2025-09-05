// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;

#if HAS_WINUI
namespace ReactiveUI.Uno.WinUI;
#else
namespace ReactiveUI.Uno;
#endif

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

        RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;

        // Use a safe default scheduler to avoid accessing platform dispatchers in headless environments.
        RxApp.MainThreadScheduler = CurrentThreadScheduler.Instance;
    }
}
