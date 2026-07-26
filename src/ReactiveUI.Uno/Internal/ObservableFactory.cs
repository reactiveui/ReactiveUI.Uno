// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM

namespace ReactiveUI.Uno.Reactive.Internal;
#else

namespace ReactiveUI.Uno.Internal;
#endif

/// <summary>Creates observables with explicit subscription state across both reactive implementations.</summary>
internal static class ObservableFactory
{
    /// <summary>Creates an observable whose subscription callback receives explicit state.</summary>
    /// <typeparam name="T">The observed value type.</typeparam>
    /// <typeparam name="TState">The subscription state type.</typeparam>
    /// <param name="state">The subscription state.</param>
    /// <param name="subscribe">The subscription callback.</param>
    /// <returns>The state-backed observable.</returns>
    internal static IObservable<T> CreateWithState<T, TState>(
        TState state,
        Func<TState, IObserver<T>, IDisposable> subscribe)
    {
        ArgumentNullException.ThrowIfNull(subscribe);

#if REACTIVE_SHIM
        return new StateObservable<T, TState>(state, subscribe);
#else
        return Observable.CreateWithState(state, subscribe);
#endif
    }

    /// <summary>Stores explicit subscription state for reactive implementations without a state overload.</summary>
    /// <typeparam name="T">The observed value type.</typeparam>
    /// <typeparam name="TState">The subscription state type.</typeparam>
    /// <param name="state">The subscription state.</param>
    /// <param name="subscribe">The subscription callback.</param>
    private sealed class StateObservable<T, TState>(
        TState state,
        Func<TState, IObserver<T>, IDisposable> subscribe) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentNullException.ThrowIfNull(observer);
            return subscribe(state, observer);
        }
    }
}
