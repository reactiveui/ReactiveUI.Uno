// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
#if REACTIVE_SHIM
using ReactiveUI.Uno.Reactive.Internal;
#else
using ReactiveUI.Uno.Internal;
#endif

#if REACTIVE_SHIM

namespace ReactiveUI.Uno.Reactive;
#else

namespace ReactiveUI.Uno;
#endif

/// <summary>
/// ActivationForViewFetcher is how ReactiveUI determine when a
/// View is activated or deactivated. This is usually only used when porting
/// ReactiveUI to a new UI framework.
/// </summary>
public class ActivationForViewFetcher : IActivationForViewFetcher
{
    /// <inheritdoc/>
    public int GetAffinityForView(Type view) => typeof(FrameworkElement).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ? 10 : 0;

    /// <inheritdoc/>
    public IObservable<bool> GetActivationForView(IActivatableView view)
    {
        if (view is not FrameworkElement fe)
        {
            return Observable.Empty<bool>();
        }

        var viewLoaded = Observable.Create<bool>(observer =>
        {
            void Handler(FrameworkElement sender, object args) => observer.OnNext(true);

            fe.Loading += Handler;
            return Disposable.Create(() => fe.Loading -= Handler);
        });

        var viewUnloaded = Observable.Create<bool>(observer =>
        {
            void Handler(object sender, RoutedEventArgs args) => observer.OnNext(false);

            fe.Unloaded += Handler;
            return Disposable.Create(() => fe.Unloaded -= Handler);
        });

        // Observe IsHitTestVisible property changes using DependencyProperty (AOT-safe)
        var isHitTestVisible = ReactiveHelpers.CreatePropertyValueObservable(
            fe,
            nameof(fe.IsHitTestVisible),
            FrameworkElement.IsHitTestVisibleProperty,
            () => fe.IsHitTestVisible);

        return viewLoaded
               .Merge(viewUnloaded)
               .Select(b => b ? isHitTestVisible.SkipWhile(x => !x) : Observable.Return(false))
               .Switch()
               .DistinctUntilChanged();
    }
}
