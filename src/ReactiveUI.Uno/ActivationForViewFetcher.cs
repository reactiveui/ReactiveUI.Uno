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

        var viewLoaded = ObservableFactory.CreateWithState<bool, FrameworkElement>(fe, static (element, observer) =>
        {
            Windows.Foundation.TypedEventHandler<FrameworkElement, object> handler =
                (_, _) => observer.OnNext(true);

            element.Loading += handler;
            return Disposable.Create(
                (Element: element, Handler: handler),
                static subscription => subscription.Element.Loading -= subscription.Handler);
        });

        var viewUnloaded = ObservableFactory.CreateWithState<bool, FrameworkElement>(fe, static (element, observer) =>
        {
            RoutedEventHandler handler = (_, _) => observer.OnNext(false);

            element.Unloaded += handler;
            return Disposable.Create(
                (Element: element, Handler: handler),
                static subscription => subscription.Element.Unloaded -= subscription.Handler);
        });

        // Observe IsHitTestVisible property changes using DependencyProperty (AOT-safe)
        var isHitTestVisible = ReactiveHelpers.CreatePropertyValueObservable(
            fe,
            nameof(fe.IsHitTestVisible),
            FrameworkElement.IsHitTestVisibleProperty,
            () => fe.IsHitTestVisible);

        return viewLoaded
               .Merge(viewUnloaded)
               .Select(b => b ? isHitTestVisible.SkipWhile(static x => !x) : Observable.Return(false))
               .Switch()
               .DistinctUntilChanged();
    }
}
