// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using System.Reflection;
using ReactiveUI.Uno.Internal;
using Windows.Foundation;

namespace ReactiveUI.Uno;

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
            return Observable<bool>.Empty;
        }

#pragma warning disable SA1114 // Parameter list after.
        var viewLoaded = Observable.FromEvent<TypedEventHandler<FrameworkElement, object>, bool>(

            eventHandler => (_, _) => eventHandler(true),
            x => fe.Loading += x,
            x => fe.Loading -= x);

        var viewUnloaded = Observable.FromEvent<RoutedEventHandler, bool>(
            handler =>
            {
                void EventHandler(object sender, RoutedEventArgs e) => handler(false);
                return EventHandler;
            },
            x => fe.Unloaded += x,
            x => fe.Unloaded -= x);

        // Observe IsHitTestVisible property changes using DependencyProperty (AOT-safe)
        var isHitTestVisible = ReactiveHelpers.CreatePropertyValueObservable(
            fe,
            nameof(fe.IsHitTestVisible),
            FrameworkElement.IsHitTestVisibleProperty,
            () => fe.IsHitTestVisible);

        return viewLoaded
               .Merge(viewUnloaded)
               .Select(b => b ? isHitTestVisible.SkipWhile(x => !x) : Observables.False)
               .Switch()
               .DistinctUntilChanged();
    }
}
