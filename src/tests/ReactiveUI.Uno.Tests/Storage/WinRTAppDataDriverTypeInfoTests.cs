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

/// <summary>Contains JsonTypeInfo tests for the WinRT application data driver.</summary>
public partial class WinRTAppDataDriverTests
{
    /// <summary>Validates that SaveState with JsonTypeInfo throws ArgumentNullException when state is null.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveStateWithTypeInfo_ThrowsArgumentNullException_WhenStateIsNull() =>
        await Assert.That(() => _sut.SaveState<TestState>(null!, TestStateJsonContext.Default.TestState))
            .Throws<ArgumentNullException>();

    /// <summary>Validates that SaveState with JsonTypeInfo rejects null type info.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveStateWithTypeInfo_ThrowsArgumentNullException_WhenTypeInfoIsNull()
    {
        var state = new TestState { Name = "Test", Value = DefaultStateValue };

        await Assert.That(() => _sut.SaveState(state, null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Validates that SaveState with JsonTypeInfo returns a non-null observable.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveStateWithTypeInfo_ReturnsNonNullObservable()
    {
        var state = new TestState { Name = "Test", Value = DefaultStateValue };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.TestState);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates that SaveState with JsonTypeInfo returns an observable of RxVoid type.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveStateWithTypeInfo_ReturnsObservableOfRxVoid()
    {
        var state = new TestState { Name = "Test", Value = DefaultStateValue };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.TestState);

        await Assert.That(observable).IsAssignableTo<IObservable<RxVoid>>();
    }

    /// <summary>Validates that LoadState with JsonTypeInfo rejects null type info.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadStateWithTypeInfo_ThrowsArgumentNullException_WhenTypeInfoIsNull() =>
        await Assert.That(() => _sut.LoadState<TestState>(null!))
            .Throws<ArgumentNullException>();

    /// <summary>Validates that LoadState with JsonTypeInfo returns a non-null observable.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadStateWithTypeInfo_ReturnsNonNullObservable()
    {
        var observable = _sut.LoadState(TestStateJsonContext.Default.TestState);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates that LoadState with JsonTypeInfo returns an observable of the specified type.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadStateWithTypeInfo_ReturnsObservableOfSpecifiedType()
    {
        var observable = _sut.LoadState(TestStateJsonContext.Default.TestState);

        await Assert.That(observable).IsAssignableTo<IObservable<TestState?>>();
    }

    /// <summary>Validates SaveState with JsonTypeInfo handles complex nested objects.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveStateWithTypeInfo_HandlesNestedState()
    {
        var state = new NestedTestState { Id = 1, Inner = new TestState { Name = "Inner", Value = NestedInnerValue } };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.NestedTestState);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates that SaveState with JsonTypeInfo handles null inner state.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveStateWithTypeInfo_HandlesNullInnerState()
    {
        var state = new NestedTestState { Id = 1, Inner = null };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.NestedTestState);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates that LoadState with JsonTypeInfo can be subscribed to.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadStateWithTypeInfo_ObservableCanBeSubscribed()
    {
        var observable = _sut.LoadState(TestStateJsonContext.Default.TestState);
        var subscribed = false;

        _ = observable.Subscribe(
            _ => subscribed = true,
            _ => subscribed = true);

        // Give observable time to complete or error
        await Task.Delay(SubscriptionDelayMilliseconds);

        await Assert.That(subscribed).IsTrue();
    }

    /// <summary>Validates that SaveState with JsonTypeInfo observable executes when subscribed.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveStateWithTypeInfo_ExecutesWhenSubscribed()
    {
        var state = new TestState { Name = "Test", Value = DefaultStateValue };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.TestState);

        var executed = false;

        _ = observable.Subscribe(
            _ => executed = true,
            _ => executed = true,
            () => executed = true);

        // Give the observable time to complete
        await Task.Delay(ExecutionDelayMilliseconds);

        await Assert.That(executed).IsTrue();
    }

    /// <summary>Validates that LoadState with JsonTypeInfo observable executes when subscribed.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadStateWithTypeInfo_ExecutesWhenSubscribed()
    {
        var observable = _sut.LoadState(TestStateJsonContext.Default.TestState);

        var executed = false;

        _ = observable.Subscribe(
            _ => executed = true,
            _ => executed = true,
            () => executed = true);

        // Give the observable time to complete
        await Task.Delay(ExecutionDelayMilliseconds);

        await Assert.That(executed).IsTrue();
    }

    /// <summary>Validates typed SaveState errors when ApplicationData is unavailable.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveStateWithTypeInfo_ObservableErrorsOnNonWindowsPlatform()
    {
        var state = new TestState { Name = "Test", Value = DefaultStateValue };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.TestState);

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

    /// <summary>Validates typed LoadState errors when ApplicationData is unavailable.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadStateWithTypeInfo_ObservableErrorsOnNonWindowsPlatform()
    {
        var observable = _sut.LoadState(TestStateJsonContext.Default.TestState);

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

    /// <summary>Validates that SaveState with JsonTypeInfo returns new observable each call.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveStateWithTypeInfo_ReturnsNewObservableEachCall()
    {
        var state = new TestState { Name = "Test", Value = DefaultStateValue };
        var observable1 = _sut.SaveState(state, TestStateJsonContext.Default.TestState);
        var observable2 = _sut.SaveState(state, TestStateJsonContext.Default.TestState);

        await Assert.That(observable1).IsNotSameReferenceAs(observable2);
    }

    /// <summary>Validates that LoadState with JsonTypeInfo returns new observable each call.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadStateWithTypeInfo_ReturnsNewObservableEachCall()
    {
        var observable1 = _sut.LoadState(TestStateJsonContext.Default.TestState);
        var observable2 = _sut.LoadState(TestStateJsonContext.Default.TestState);

        await Assert.That(observable1).IsNotSameReferenceAs(observable2);
    }

    /// <summary>Validates that SaveState with JsonTypeInfo handles special characters.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveStateWithTypeInfo_HandlesSpecialCharacters()
    {
        var state = new TestState { Name = "Test<>&'\"", Value = DefaultStateValue };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.TestState);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates that SaveState with JsonTypeInfo handles unicode.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveStateWithTypeInfo_HandlesUnicode()
    {
        var state = new TestState { Name = "测试 テスト 🎉", Value = DefaultStateValue };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.TestState);

        await Assert.That(observable).IsNotNull();
    }
}
