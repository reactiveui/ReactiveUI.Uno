// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
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
    /// <summary>The affinity assigned to framework element views.</summary>
    private const int FrameworkElementAffinity = 10;

    /// <inheritdoc/>
    public int GetAffinityForView(Type view) =>
        typeof(FrameworkElement).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo())
            ? FrameworkElementAffinity
            : 0;

    /// <inheritdoc/>
    public IObservable<bool> GetActivationForView(IActivatableView view)
    {
        if (view is not FrameworkElement fe)
        {
            return Observable.Empty<bool>();
        }

        var viewLoaded = Observable.Create<bool>(observer =>
        {
            Windows.Foundation.TypedEventHandler<FrameworkElement, object> handler =
                (_, _) => observer.OnNext(true);

            fe.Loading += handler;
            return Disposable.Create(() => fe.Loading -= handler);
        });

        var viewUnloaded = Observable.Create<bool>(observer =>
        {
            RoutedEventHandler handler = (_, _) => observer.OnNext(false);

            fe.Unloaded += handler;
            return Disposable.Create(() => fe.Unloaded -= handler);
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
