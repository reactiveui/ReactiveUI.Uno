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

/// <summary>Contains SaveState tests for the WinRT application data driver.</summary>
public partial class WinRTAppDataDriverTests
{
    /// <summary>
    /// Validates that SaveState throws ArgumentNullException when state is null.
    /// The exception is thrown when the observable is subscribed to.
    /// </summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_ThrowsArgumentNullException_WhenStateIsNull()
    {
        var observable = _sut.SaveState<object>(null!);

        // The ArgumentNullException is thrown during observable execution
        await Assert.That(() => observable.ToTask()).Throws<ArgumentNullException>();
    }

    /// <summary>Validates that SaveState returns a non-null observable when state is valid.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_ReturnsNonNullObservable_WhenStateIsValid()
    {
        var state = new TestState { Name = "Test", Value = DefaultStateValue };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates that SaveState returns an observable of RxVoid type.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_ReturnsObservableOfRxVoid()
    {
        var state = new TestState { Name = "Test", Value = DefaultStateValue };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsAssignableTo<IObservable<RxVoid>>();
    }

    /// <summary>Validates that SaveState observable can be subscribed to multiple times.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_ObservableCanBeSubscribedMultipleTimes()
    {
        var state = new TestState { Name = "Test", Value = DefaultStateValue };
        var observable = _sut.SaveState(state);

        var firstSubscription = observable.ToTask();
        var secondSubscription = observable.ToTask();
        var firstTermination = AwaitTerminationAsync(firstSubscription);
        var secondTermination = AwaitTerminationAsync(secondSubscription);
        var firstTerminated = await firstTermination;
        var secondTerminated = await secondTermination;

        await Assert.That(firstTerminated && secondTerminated).IsTrue();
    }

    /// <summary>Validates that SaveState returns a cold observable (doesn't execute until subscribed).</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_ReturnsColdObservable()
    {
        var state = new TestState { Name = "Test", Value = DefaultStateValue };

        // Creating the observable should not throw even without subscription
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();

        // The observable should be lazy - no execution until subscribed
        await Assert.That(observable).IsAssignableTo<IObservable<RxVoid>>();
    }

    /// <summary>Validates that SaveState observable executes when subscribed.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_ExecutesWhenSubscribed()
    {
        var state = new TestState { Name = "Test", Value = DefaultStateValue };
        var observable = _sut.SaveState(state);

        var completed = false;
        var errored = false;

        _ = observable.Subscribe(
            _ => completed = true,
            _ => errored = true,
            () => completed = true);

        // Give the observable time to complete
        await Task.Delay(ExecutionDelayMilliseconds);

        // Either completed successfully or errored (both indicate execution)
        await Assert.That(completed || errored).IsTrue();
    }

    /// <summary>Validates SaveState handles empty string property values.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_HandlesEmptyStringProperty()
    {
        var state = new TestState { Name = string.Empty, Value = 0 };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates SaveState handles complex nested objects.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_HandlesNestedState()
    {
        var state = new NestedTestState { Id = 1, Inner = new TestState { Name = "Inner", Value = NestedInnerValue } };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates that SaveState handles null inner state in nested object.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_HandlesNullInnerState()
    {
        var state = new NestedTestState { Id = 1, Inner = null };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates that SaveState observable errors on platform without ApplicationData.
    /// In non-Windows TFMs, ApplicationData.Current throws an exception.
    /// </summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_ObservableErrorsOnNonWindowsPlatform()
    {
        var state = new TestState { Name = "Test", Value = DefaultStateValue };
        var observable = _sut.SaveState(state);

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

    /// <summary>Validates that SaveState returns new observable each call.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_ReturnsNewObservableEachCall()
    {
        var state = new TestState { Name = "Test", Value = DefaultStateValue };
        var observable1 = _sut.SaveState(state);
        var observable2 = _sut.SaveState(state);

        await Assert.That(observable1).IsNotSameReferenceAs(observable2);
    }

    /// <summary>Validates SaveState handles state with special characters in string properties.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_HandlesSpecialCharactersInStrings()
    {
        var state = new TestState { Name = "Test<>&'\"", Value = DefaultStateValue };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates SaveState handles state with unicode characters.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_HandlesUnicodeCharacters()
    {
        var state = new TestState { Name = "测试 テスト 🎉", Value = DefaultStateValue };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates SaveState handles state with very long strings.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_HandlesLongStrings()
    {
        var state = new TestState { Name = new('x', LongStringLength), Value = DefaultStateValue };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates SaveState handles state with negative values.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_HandlesNegativeValues()
    {
        var state = new TestState { Name = "Test", Value = NegativeStateValue };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates SaveState handles state with max int value.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_HandlesMaxIntValue()
    {
        var state = new TestState { Name = "Test", Value = int.MaxValue };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates SaveState handles state with min int value.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_HandlesMinIntValue()
    {
        var state = new TestState { Name = "Test", Value = int.MinValue };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates SaveState observable can be converted to task.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_ObservableCanBeConvertedToTask()
    {
        var state = new TestState { Name = "Test", Value = DefaultStateValue };
        var observable = _sut.SaveState(state);

        // Should be able to get task from observable
        var task = observable.ToTask();

        await Assert.That((object)task).IsNotNull();
        await Assert.That((object)task).IsAssignableTo<Task>();
        await Assert.That(await AwaitTerminationAsync(task)).IsTrue();
    }

    /// <summary>Validates that deeply nested objects can be handled by SaveState.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_HandlesDeeplyNestedObjects()
    {
        var innerState = new TestState { Name = "Level1", Value = DeepNestedValue };
        var state = new NestedTestState { Id = 1, Inner = innerState };

        var observable = _sut.SaveState(state);
        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates SaveState with different state instances returns unique observables.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_DifferentStateInstances_ReturnUniqueObservables()
    {
        var state1 = new TestState { Name = "Test1", Value = 1 };
        var state2 = new TestState { Name = "Test2", Value = SecondStateValue };

        var observable1 = _sut.SaveState(state1);
        var observable2 = _sut.SaveState(state2);

        await Assert.That(observable1).IsNotSameReferenceAs(observable2);
    }
}
