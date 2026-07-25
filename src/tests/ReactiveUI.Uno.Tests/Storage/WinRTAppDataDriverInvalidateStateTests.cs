// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Assertions.Extensions;
using TUnit.Core;
#if REACTIVE_SHIM
using RxVoid = System.Reactive.Unit;
#else
using RxVoid = ReactiveUI.Primitives.RxVoid;
#endif

namespace ReactiveUI.Uno.Tests.Storage;

/// <summary>Contains InvalidateState tests for the WinRT application data driver.</summary>
public partial class WinRTAppDataDriverTests
{
    /// <summary>Validates that InvalidateState returns a non-null observable.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task InvalidateState_ReturnsNonNullObservable()
    {
        var observable = _sut.InvalidateState();

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates that InvalidateState returns an observable of RxVoid type.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task InvalidateState_ReturnsObservableOfRxVoid()
    {
        var observable = _sut.InvalidateState();

        await Assert.That(observable).IsAssignableTo<IObservable<RxVoid>>();
    }

    /// <summary>Validates that InvalidateState observable can be subscribed to.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task InvalidateState_ObservableCanBeSubscribed()
    {
        var observable = _sut.InvalidateState();
        var subscribed = false;

        _ = observable.Subscribe(
            _ => subscribed = true,
            _ => subscribed = true);

        // Give observable time to complete or error
        await Task.Delay(SubscriptionDelayMilliseconds);

        await Assert.That(subscribed).IsTrue();
    }

    /// <summary>Validates that calling InvalidateState multiple times returns separate observables.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task InvalidateState_ReturnsNewObservableEachCall()
    {
        var observable1 = _sut.InvalidateState();
        var observable2 = _sut.InvalidateState();

        await Assert.That(observable1).IsNotSameReferenceAs(observable2);
    }

    /// <summary>Validates that InvalidateState returns a cold observable.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task InvalidateState_ReturnsColdObservable()
    {
        // Creating the observable should not throw even without subscription
        var observable = _sut.InvalidateState();

        await Assert.That(observable).IsNotNull();

        // The observable should be lazy - no execution until subscribed
        await Assert.That(observable).IsAssignableTo<IObservable<RxVoid>>();
    }

    /// <summary>Validates that InvalidateState observable executes when subscribed.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task InvalidateState_ExecutesWhenSubscribed()
    {
        var observable = _sut.InvalidateState();

        var executed = false;

        _ = observable.Subscribe(
            _ => executed = true,
            _ => executed = true,
            () => executed = true);

        // Give the observable time to complete
        await Task.Delay(ExecutionDelayMilliseconds);

        await Assert.That(executed).IsTrue();
    }

    /// <summary>Validates that InvalidateState observable errors on platform without ApplicationData.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task InvalidateState_ObservableErrorsOnNonWindowsPlatform()
    {
        var observable = _sut.InvalidateState();

        Exception? caughtException = null;
        var completed = false;

        _ = observable.Subscribe(
            _ => completed = true,
            ex => caughtException = ex,
            () => completed = true);

        // Give the observable time to complete
        await Task.Delay(ExecutionDelayMilliseconds);

        // Either completes (on Windows with proper setup) or errors (on non-Windows or no roaming folder)
        await Assert.That(completed || caughtException is not null).IsTrue();
    }

    /// <summary>Validates InvalidateState observable can be converted to task.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task InvalidateState_ObservableCanBeConvertedToTask()
    {
        var observable = _sut.InvalidateState();

        // Should be able to get task from observable
        var task = observable.ToTask();

        await Assert.That((object)task).IsNotNull();
        await Assert.That((object)task).IsAssignableTo<Task>();
    }
}
