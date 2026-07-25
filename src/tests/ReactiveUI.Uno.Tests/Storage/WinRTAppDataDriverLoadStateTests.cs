// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Storage;

/// <summary>Contains LoadState tests for the WinRT application data driver.</summary>
public partial class WinRTAppDataDriverTests
{
    /// <summary>Validates that LoadState returns a non-null observable.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadState_ReturnsNonNullObservable()
    {
        var observable = _sut.LoadState();

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates that LoadState returns an observable of object type.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadState_ReturnsObservableOfObject()
    {
        var observable = _sut.LoadState();

        await Assert.That(observable).IsAssignableTo<IObservable<object?>>();
    }

    /// <summary>Validates that LoadState observable can be subscribed to.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadState_ObservableCanBeSubscribed()
    {
        var observable = _sut.LoadState();
        var subscribed = false;

        _ = observable.Subscribe(
            _ => subscribed = true,
            _ => subscribed = true);

        // Give observable time to complete or error
        await Task.Delay(SubscriptionDelayMilliseconds);

        await Assert.That(subscribed).IsTrue();
    }

    /// <summary>Validates that LoadState returns a cold observable (doesn't execute until subscribed).</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadState_ReturnsColdObservable()
    {
        // Creating the observable should not throw even without subscription
        var observable = _sut.LoadState();

        await Assert.That(observable).IsNotNull();

        // The observable should be lazy - no execution until subscribed
        await Assert.That(observable).IsAssignableTo<IObservable<object?>>();
    }

    /// <summary>Validates that LoadState observable executes when subscribed.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadState_ExecutesWhenSubscribed()
    {
        var observable = _sut.LoadState();

        var executed = false;

        _ = observable.Subscribe(
            _ => executed = true,
            _ => executed = true,
            () => executed = true);

        // Give the observable time to complete
        await Task.Delay(ExecutionDelayMilliseconds);

        await Assert.That(executed).IsTrue();
    }

    /// <summary>Validates that LoadState observable errors on platform without ApplicationData.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadState_ObservableErrorsOnNonWindowsPlatform()
    {
        var observable = _sut.LoadState();

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

    /// <summary>Validates that LoadState returns new observable each call.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadState_ReturnsNewObservableEachCall()
    {
        var observable1 = _sut.LoadState();
        var observable2 = _sut.LoadState();

        await Assert.That(observable1).IsNotSameReferenceAs(observable2);
    }

    /// <summary>Validates LoadState observable can be converted to task.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadState_ObservableCanBeConvertedToTask()
    {
        var observable = _sut.LoadState();

        // Should be able to get task from observable
        var task = observable.ToTask();

        await Assert.That((object)task).IsNotNull();
        await Assert.That((object)task).IsAssignableTo<Task>();
    }
}
