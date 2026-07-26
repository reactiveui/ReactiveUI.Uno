// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

#if REACTIVE_SHIM

namespace ReactiveUI.Uno.Reactive.Internal;
#else

namespace ReactiveUI.Uno.Internal;
#endif

/// <summary>Provides AOT-friendly reactive helper methods for Uno controls.</summary>
internal static class ReactiveHelpers
{
    /// <summary>
    /// Creates an observable that emits when the specified property changes on the source object.
    /// Uses PropertyChanged event directly without expression trees, making it AOT-compatible.
    /// </summary>
    /// <param name="source">The object to observe.</param>
    /// <param name="propertyName">The name of the property to observe (use nameof()).</param>
    /// <returns>An observable that emits Unit when the property changes.</returns>
    /// <remarks>
    /// This method uses Observable.Create for better performance compared to Observable.FromEvent.
    /// It filters PropertyChanged events to only emit when the specified property changes.
    /// </remarks>
    internal static IObservable<Unit> CreatePropertyChangedPulse(
        INotifyPropertyChanged source,
        string propertyName)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(propertyName);

        return ObservableFactory.CreateWithState<Unit, (INotifyPropertyChanged Source, string PropertyName)>(
            (source, propertyName),
            static (state, observer) =>
        {
            void Handler(object? _, PropertyChangedEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.PropertyName)
                    && !string.Equals(e.PropertyName, state.PropertyName, StringComparison.Ordinal))
                {
                    return;
                }

                observer.OnNext(Unit.Default);
            }

            state.Source.PropertyChanged += Handler;
            return Disposable.Create<(INotifyPropertyChanged Source, PropertyChangedEventHandler Handler)>(
                (state.Source, Handler),
                static subscription => subscription.Source.PropertyChanged -= subscription.Handler);
        });
    }

    /// <summary>
    /// Creates an observable that emits the current value of a property whenever it changes.
    /// Uses PropertyChanged event directly without expression trees, making it AOT-compatible.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="source">The object to observe (must implement INotifyPropertyChanged).</param>
    /// <param name="propertyName">The name of the property to observe (use nameof()).</param>
    /// <param name="getPropertyValue">A function to retrieve the current property value.</param>
    /// <returns>An observable that emits the property value when it changes.</returns>
    /// <remarks>
    /// This provides an AOT-friendly alternative to WhenAnyValue by avoiding expression trees.
    /// The observable immediately emits the current value upon subscription, then emits whenever the property changes.
    /// This overload works with any INotifyPropertyChanged implementation and is available for MAUI.
    /// </remarks>
    internal static IObservable<T> CreatePropertyValueObservable<T>(
        INotifyPropertyChanged source,
        string propertyName,
        Func<T> getPropertyValue)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(getPropertyValue);

        return ObservableFactory.CreateWithState<T, (INotifyPropertyChanged Source, string PropertyName, Func<T> GetValue)>(
            (source, propertyName, getPropertyValue),
            static (state, observer) =>
        {
            // Emit initial value
            observer.OnNext(state.GetValue());

            void Handler(object? _, PropertyChangedEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.PropertyName)
                    && !string.Equals(e.PropertyName, state.PropertyName, StringComparison.Ordinal))
                {
                    return;
                }

                observer.OnNext(state.GetValue());
            }

            state.Source.PropertyChanged += Handler;
            return Disposable.Create<(INotifyPropertyChanged Source, PropertyChangedEventHandler Handler)>(
                (state.Source, Handler),
                static subscription => subscription.Source.PropertyChanged -= subscription.Handler);
        });
    }

    /// <summary>
    /// Creates an observable that emits the current value of a DependencyProperty whenever it changes.
    /// This is a WinUI-specific overload that avoids reflection by accepting the DependencyProperty directly.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="source">The DependencyObject to observe.</param>
    /// <param name="propertyName">The name of the property to observe (use nameof()).</param>
    /// <param name="property">The DependencyProperty to observe.</param>
    /// <param name="getPropertyValue">A function to retrieve the current property value.</param>
    /// <returns>An observable that emits the property value when it changes.</returns>
    /// <remarks>
    /// This provides an AOT-friendly alternative to WhenAnyValue by avoiding expression trees and reflection.
    /// The observable immediately emits the current value upon subscription, then emits whenever the property changes.
    /// </remarks>
    internal static IObservable<T> CreatePropertyValueObservable<T>(
        DependencyObject source,
        string propertyName,
        DependencyProperty property,
        Func<T> getPropertyValue)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(getPropertyValue);

        return ObservableFactory.CreateWithState<T, (DependencyObject Source, DependencyProperty Property, Func<T> GetValue)>(
            (source, property, getPropertyValue),
            static (state, observer) =>
        {
            // Emit initial value
            observer.OnNext(state.GetValue());

            // Register for property changes using the provided DependencyProperty
            var token = state.Source.RegisterPropertyChangedCallback(
                state.Property,
                (_, _) => observer.OnNext(state.GetValue()));

            return Disposable.Create(
                (state.Source, state.Property, Token: token),
                static subscription => subscription.Source.UnregisterPropertyChangedCallback(
                    subscription.Property,
                    subscription.Token));
        });
    }

    /// <summary>Wires up activation for a view model that supports activation.</summary>
    /// <param name="viewModel">The view model to activate.</param>
    /// <param name="activatedSignal">Observable that signals when the view is activated.</param>
    /// <param name="deactivatedSignal">Observable that signals when the view is deactivated.</param>
    /// <returns>A disposable that manages the activation subscriptions.</returns>
    internal static IDisposable WireActivationIfSupported(
        object? viewModel,
        IObservable<Unit> activatedSignal,
        IObservable<Unit> deactivatedSignal)
    {
        if (viewModel is not IActivatableViewModel activatable)
        {
            return Disposable.Empty;
        }

        var activatedSub = activatedSignal.Subscribe(_ => activatable.Activator.Activate());
        var deactivatedSub = deactivatedSignal.Subscribe(_ => activatable.Activator.Deactivate());

        return new CompositeDisposable(activatedSub, deactivatedSub);
    }
}
