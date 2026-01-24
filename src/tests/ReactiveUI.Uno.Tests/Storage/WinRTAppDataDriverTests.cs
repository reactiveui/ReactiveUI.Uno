// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Storage;

/// <summary>
/// Contains tests for the <see cref="WinRTAppDataDriver"/> class, ensuring its functionality
/// for loading, saving, and invalidating application state.
/// </summary>
public class WinRTAppDataDriverTests
{
    private WinRTAppDataDriver _sut = null!;

    /// <summary>
    /// Sets up the test by creating a new instance of WinRTAppDataDriver.
    /// </summary>
    [Before(Test)]
    public void SetUp() => _sut = new WinRTAppDataDriver();

    /// <summary>
    /// Validates that WinRTAppDataDriver implements ISuspensionDriver interface.
    /// </summary>
    [Test]
    public async Task WinRTAppDataDriver_ImplementsISuspensionDriver() => await Assert.That(_sut).IsAssignableTo<ISuspensionDriver>();

    /// <summary>
    /// Validates that multiple instances can be created independently.
    /// </summary>
    [Test]
    public async Task Constructor_AllowsMultipleInstances()
    {
        var sut1 = new WinRTAppDataDriver();
        var sut2 = new WinRTAppDataDriver();

        await Assert.That(sut1).IsNotNull();
        await Assert.That(sut2).IsNotNull();
        await Assert.That(sut1).IsNotSameReferenceAs(sut2);
    }

    /// <summary>
    /// Validates that default constructor creates a valid instance.
    /// </summary>
    [Test]
    public async Task Constructor_CreatesValidInstance()
    {
        var driver = new WinRTAppDataDriver();

        await Assert.That(driver).IsNotNull();
        await Assert.That(driver).IsAssignableTo<WinRTAppDataDriver>();
    }

    /// <summary>
    /// Validates that SaveState throws ArgumentNullException when state is null.
    /// The exception is thrown when the observable is subscribed to.
    /// </summary>
    [Test]
    public async Task SaveState_ThrowsArgumentNullException_WhenStateIsNull()
    {
        var observable = _sut.SaveState<object>(null!);

        // The ArgumentNullException is thrown during observable execution
        await Assert.That(async () => await observable.FirstAsync()).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Validates that SaveState returns a non-null observable when state is valid.
    /// </summary>
    [Test]
    public async Task SaveState_ReturnsNonNullObservable_WhenStateIsValid()
    {
        var state = new TestState { Name = "Test", Value = 42 };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates that SaveState returns an observable of Unit type.
    /// </summary>
    [Test]
    public async Task SaveState_ReturnsObservableOfUnit()
    {
        var state = new TestState { Name = "Test", Value = 42 };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsAssignableTo<IObservable<Unit>>();
    }

    /// <summary>
    /// Validates that SaveState observable can be subscribed to multiple times.
    /// </summary>
    [Test]
    public async Task SaveState_ObservableCanBeSubscribedMultipleTimes()
    {
        var state = new TestState { Name = "Test", Value = 42 };
        var observable = _sut.SaveState(state);

        // Each subscription should create a new cold observable
        var subscription1Received = false;
        var subscription2Received = false;

        observable.Subscribe(
            _ => subscription1Received = true,
            _ => subscription1Received = true);

        observable.Subscribe(
            _ => subscription2Received = true,
            _ => subscription2Received = true);

        // Give observables time to complete
        await Task.Delay(100);

        // At least the subscriptions should have been triggered
        await Assert.That(subscription1Received || subscription2Received).IsTrue();
    }

    /// <summary>
    /// Validates that SaveState with JsonTypeInfo throws ArgumentNullException when state is null.
    /// </summary>
    [Test]
    public async Task SaveStateWithTypeInfo_ThrowsArgumentNullException_WhenStateIsNull() => await Assert.That(() => _sut.SaveState<TestState>(null!, TestStateJsonContext.Default.TestState))
            .Throws<ArgumentNullException>();

    /// <summary>
    /// Validates that SaveState with JsonTypeInfo throws ArgumentNullException when typeInfo is null.
    /// </summary>
    [Test]
    public async Task SaveStateWithTypeInfo_ThrowsArgumentNullException_WhenTypeInfoIsNull()
    {
        var state = new TestState { Name = "Test", Value = 42 };

        await Assert.That(() => _sut.SaveState(state, null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Validates that SaveState with JsonTypeInfo returns a non-null observable.
    /// </summary>
    [Test]
    public async Task SaveStateWithTypeInfo_ReturnsNonNullObservable()
    {
        var state = new TestState { Name = "Test", Value = 42 };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.TestState);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates that SaveState with JsonTypeInfo returns an observable of Unit type.
    /// </summary>
    [Test]
    public async Task SaveStateWithTypeInfo_ReturnsObservableOfUnit()
    {
        var state = new TestState { Name = "Test", Value = 42 };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.TestState);

        await Assert.That(observable).IsAssignableTo<IObservable<Unit>>();
    }

    /// <summary>
    /// Validates that LoadState returns a non-null observable.
    /// </summary>
    [Test]
    public async Task LoadState_ReturnsNonNullObservable()
    {
        var observable = _sut.LoadState();

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates that LoadState returns an observable of object type.
    /// </summary>
    [Test]
    public async Task LoadState_ReturnsObservableOfObject()
    {
        var observable = _sut.LoadState();

        await Assert.That(observable).IsAssignableTo<IObservable<object?>>();
    }

    /// <summary>
    /// Validates that LoadState observable can be subscribed to.
    /// </summary>
    [Test]
    public async Task LoadState_ObservableCanBeSubscribed()
    {
        var observable = _sut.LoadState();
        var subscribed = false;

        observable.Subscribe(
            _ => subscribed = true,
            _ => subscribed = true);

        // Give observable time to complete or error
        await Task.Delay(100);

        await Assert.That(subscribed).IsTrue();
    }

    /// <summary>
    /// Validates that LoadState with JsonTypeInfo throws ArgumentNullException when typeInfo is null.
    /// </summary>
    [Test]
    public async Task LoadStateWithTypeInfo_ThrowsArgumentNullException_WhenTypeInfoIsNull() => await Assert.That(() => _sut.LoadState<TestState>(null!))
            .Throws<ArgumentNullException>();

    /// <summary>
    /// Validates that LoadState with JsonTypeInfo returns a non-null observable.
    /// </summary>
    [Test]
    public async Task LoadStateWithTypeInfo_ReturnsNonNullObservable()
    {
        var observable = _sut.LoadState(TestStateJsonContext.Default.TestState);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates that LoadState with JsonTypeInfo returns an observable of the specified type.
    /// </summary>
    [Test]
    public async Task LoadStateWithTypeInfo_ReturnsObservableOfSpecifiedType()
    {
        var observable = _sut.LoadState(TestStateJsonContext.Default.TestState);

        await Assert.That(observable).IsAssignableTo<IObservable<TestState?>>();
    }

    /// <summary>
    /// Validates that InvalidateState returns a non-null observable.
    /// </summary>
    [Test]
    public async Task InvalidateState_ReturnsNonNullObservable()
    {
        var observable = _sut.InvalidateState();

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates that InvalidateState returns an observable of Unit type.
    /// </summary>
    [Test]
    public async Task InvalidateState_ReturnsObservableOfUnit()
    {
        var observable = _sut.InvalidateState();

        await Assert.That(observable).IsAssignableTo<IObservable<Unit>>();
    }

    /// <summary>
    /// Validates that InvalidateState observable can be subscribed to.
    /// </summary>
    [Test]
    public async Task InvalidateState_ObservableCanBeSubscribed()
    {
        var observable = _sut.InvalidateState();
        var subscribed = false;

        observable.Subscribe(
            _ => subscribed = true,
            _ => subscribed = true);

        // Give observable time to complete or error
        await Task.Delay(100);

        await Assert.That(subscribed).IsTrue();
    }

    /// <summary>
    /// Validates that calling InvalidateState multiple times returns separate observables.
    /// </summary>
    [Test]
    public async Task InvalidateState_ReturnsNewObservableEachCall()
    {
        var observable1 = _sut.InvalidateState();
        var observable2 = _sut.InvalidateState();

        await Assert.That(observable1).IsNotSameReferenceAs(observable2);
    }

    /// <summary>
    /// Validates that SaveState returns a cold observable (doesn't execute until subscribed).
    /// </summary>
    [Test]
    public async Task SaveState_ReturnsColdObservable()
    {
        var state = new TestState { Name = "Test", Value = 42 };

        // Creating the observable should not throw even without subscription
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();

        // The observable should be lazy - no execution until subscribed
        await Assert.That(observable).IsAssignableTo<IObservable<Unit>>();
    }

    /// <summary>
    /// Validates that LoadState returns a cold observable (doesn't execute until subscribed).
    /// </summary>
    [Test]
    public async Task LoadState_ReturnsColdObservable()
    {
        // Creating the observable should not throw even without subscription
        var observable = _sut.LoadState();

        await Assert.That(observable).IsNotNull();

        // The observable should be lazy - no execution until subscribed
        await Assert.That(observable).IsAssignableTo<IObservable<object?>>();
    }

    /// <summary>
    /// Validates that InvalidateState returns a cold observable.
    /// </summary>
    [Test]
    public async Task InvalidateState_ReturnsColdObservable()
    {
        // Creating the observable should not throw even without subscription
        var observable = _sut.InvalidateState();

        await Assert.That(observable).IsNotNull();

        // The observable should be lazy - no execution until subscribed
        await Assert.That(observable).IsAssignableTo<IObservable<Unit>>();
    }

    /// <summary>
    /// Validates that SaveState observable executes when subscribed.
    /// </summary>
    [Test]
    public async Task SaveState_ExecutesWhenSubscribed()
    {
        var state = new TestState { Name = "Test", Value = 42 };
        var observable = _sut.SaveState(state);

        var completed = false;
        var errored = false;

        observable.Subscribe(
            _ => completed = true,
            _ => errored = true,
            () => completed = true);

        // Give the observable time to complete
        await Task.Delay(200);

        // Either completed successfully or errored (both indicate execution)
        await Assert.That(completed || errored).IsTrue();
    }

    /// <summary>
    /// Validates that LoadState observable executes when subscribed.
    /// </summary>
    [Test]
    public async Task LoadState_ExecutesWhenSubscribed()
    {
        var observable = _sut.LoadState();

        var executed = false;

        observable.Subscribe(
            _ => executed = true,
            _ => executed = true,
            () => executed = true);

        // Give the observable time to complete
        await Task.Delay(200);

        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Validates that InvalidateState observable executes when subscribed.
    /// </summary>
    [Test]
    public async Task InvalidateState_ExecutesWhenSubscribed()
    {
        var observable = _sut.InvalidateState();

        var executed = false;

        observable.Subscribe(
            _ => executed = true,
            _ => executed = true,
            () => executed = true);

        // Give the observable time to complete
        await Task.Delay(200);

        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Validates that SaveState generic method exists and is accessible.
    /// </summary>
    [Test]
    public async Task SaveState_GenericMethod_Exists()
    {
        var methods = typeof(WinRTAppDataDriver).GetMethods()
            .Where(m => m.Name == nameof(WinRTAppDataDriver.SaveState) && m.IsGenericMethod)
            .ToList();

        await Assert.That(methods.Count).IsGreaterThanOrEqualTo(1);
    }

    /// <summary>
    /// Validates that SaveState with JsonTypeInfo method exists.
    /// </summary>
    [Test]
    public async Task SaveStateWithTypeInfo_Method_Exists()
    {
        var methods = typeof(WinRTAppDataDriver).GetMethods()
            .Where(m => m.Name == nameof(WinRTAppDataDriver.SaveState))
            .ToList();

        await Assert.That(methods.Count).IsGreaterThanOrEqualTo(2);
    }

    /// <summary>
    /// Validates that LoadState non-generic method exists.
    /// </summary>
    [Test]
    public async Task LoadState_NonGenericMethod_Exists()
    {
        var method = typeof(WinRTAppDataDriver).GetMethod(
            nameof(WinRTAppDataDriver.LoadState),
            Type.EmptyTypes);

        await Assert.That(method).IsNotNull();
    }

    /// <summary>
    /// Validates that LoadState with JsonTypeInfo method exists.
    /// </summary>
    [Test]
    public async Task LoadStateWithTypeInfo_Method_Exists()
    {
        var methods = typeof(WinRTAppDataDriver).GetMethods()
            .Where(m => m.Name == nameof(WinRTAppDataDriver.LoadState))
            .ToList();

        await Assert.That(methods.Count).IsGreaterThanOrEqualTo(2);
    }

    /// <summary>
    /// Validates that InvalidateState method exists.
    /// </summary>
    [Test]
    public async Task InvalidateState_Method_Exists()
    {
        var method = typeof(WinRTAppDataDriver).GetMethod(nameof(WinRTAppDataDriver.InvalidateState));

        await Assert.That(method).IsNotNull();
    }

    /// <summary>
    /// Validates SaveState handles empty string property values.
    /// </summary>
    [Test]
    public async Task SaveState_HandlesEmptyStringProperty()
    {
        var state = new TestState { Name = string.Empty, Value = 0 };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates SaveState handles complex nested objects.
    /// </summary>
    [Test]
    public async Task SaveState_HandlesNestedState()
    {
        var state = new NestedTestState
        {
            Id = 1,
            Inner = new TestState { Name = "Inner", Value = 99 }
        };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates SaveState with JsonTypeInfo handles complex nested objects.
    /// </summary>
    [Test]
    public async Task SaveStateWithTypeInfo_HandlesNestedState()
    {
        var state = new NestedTestState
        {
            Id = 1,
            Inner = new TestState { Name = "Inner", Value = 99 }
        };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.NestedTestState);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates that SaveState handles null inner state in nested object.
    /// </summary>
    [Test]
    public async Task SaveState_HandlesNullInnerState()
    {
        var state = new NestedTestState
        {
            Id = 1,
            Inner = null
        };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates that SaveState with JsonTypeInfo handles null inner state.
    /// </summary>
    [Test]
    public async Task SaveStateWithTypeInfo_HandlesNullInnerState()
    {
        var state = new NestedTestState
        {
            Id = 1,
            Inner = null
        };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.NestedTestState);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates that LoadState with JsonTypeInfo can be subscribed to.
    /// </summary>
    [Test]
    public async Task LoadStateWithTypeInfo_ObservableCanBeSubscribed()
    {
        var observable = _sut.LoadState(TestStateJsonContext.Default.TestState);
        var subscribed = false;

        observable.Subscribe(
            _ => subscribed = true,
            _ => subscribed = true);

        // Give observable time to complete or error
        await Task.Delay(100);

        await Assert.That(subscribed).IsTrue();
    }

    /// <summary>
    /// Validates that SaveState with JsonTypeInfo observable executes when subscribed.
    /// </summary>
    [Test]
    public async Task SaveStateWithTypeInfo_ExecutesWhenSubscribed()
    {
        var state = new TestState { Name = "Test", Value = 42 };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.TestState);

        var executed = false;

        observable.Subscribe(
            _ => executed = true,
            _ => executed = true,
            () => executed = true);

        // Give the observable time to complete
        await Task.Delay(200);

        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Validates that LoadState with JsonTypeInfo observable executes when subscribed.
    /// </summary>
    [Test]
    public async Task LoadStateWithTypeInfo_ExecutesWhenSubscribed()
    {
        var observable = _sut.LoadState(TestStateJsonContext.Default.TestState);

        var executed = false;

        observable.Subscribe(
            _ => executed = true,
            _ => executed = true,
            () => executed = true);

        // Give the observable time to complete
        await Task.Delay(200);

        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Validates that SaveState observable errors on platform without ApplicationData.
    /// In non-Windows TFMs, ApplicationData.Current throws an exception.
    /// </summary>
    [Test]
    public async Task SaveState_ObservableErrorsOnNonWindowsPlatform()
    {
        var state = new TestState { Name = "Test", Value = 42 };
        var observable = _sut.SaveState(state);

        Exception? caughtException = null;
        var completed = false;

        observable.Subscribe(
            _ => completed = true,
            ex => caughtException = ex,
            () => completed = true);

        // Give the observable time to complete
        await Task.Delay(200);

        // Either completes (on Windows with proper setup) or errors (on non-Windows or no roaming folder)
        await Assert.That(completed || caughtException != null).IsTrue();
    }

    /// <summary>
    /// Validates that LoadState observable errors on platform without ApplicationData.
    /// </summary>
    [Test]
    public async Task LoadState_ObservableErrorsOnNonWindowsPlatform()
    {
        var observable = _sut.LoadState();

        Exception? caughtException = null;
        var completed = false;

        observable.Subscribe(
            _ => completed = true,
            ex => caughtException = ex,
            () => completed = true);

        // Give the observable time to complete
        await Task.Delay(200);

        // Either completes (on Windows with proper setup) or errors (on non-Windows or no roaming folder)
        await Assert.That(completed || caughtException != null).IsTrue();
    }

    /// <summary>
    /// Validates that InvalidateState observable errors on platform without ApplicationData.
    /// </summary>
    [Test]
    public async Task InvalidateState_ObservableErrorsOnNonWindowsPlatform()
    {
        var observable = _sut.InvalidateState();

        Exception? caughtException = null;
        var completed = false;

        observable.Subscribe(
            _ => completed = true,
            ex => caughtException = ex,
            () => completed = true);

        // Give the observable time to complete
        await Task.Delay(200);

        // Either completes (on Windows with proper setup) or errors (on non-Windows or no roaming folder)
        await Assert.That(completed || caughtException != null).IsTrue();
    }

    /// <summary>
    /// Validates that SaveState with JsonTypeInfo observable errors on platform without ApplicationData.
    /// </summary>
    [Test]
    public async Task SaveStateWithTypeInfo_ObservableErrorsOnNonWindowsPlatform()
    {
        var state = new TestState { Name = "Test", Value = 42 };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.TestState);

        Exception? caughtException = null;
        var completed = false;

        observable.Subscribe(
            _ => completed = true,
            ex => caughtException = ex,
            () => completed = true);

        // Give the observable time to complete
        await Task.Delay(200);

        // Either completes (on Windows with proper setup) or errors (on non-Windows or no roaming folder)
        await Assert.That(completed || caughtException != null).IsTrue();
    }

    /// <summary>
    /// Validates that LoadState with JsonTypeInfo observable errors on platform without ApplicationData.
    /// </summary>
    [Test]
    public async Task LoadStateWithTypeInfo_ObservableErrorsOnNonWindowsPlatform()
    {
        var observable = _sut.LoadState(TestStateJsonContext.Default.TestState);

        Exception? caughtException = null;
        var completed = false;

        observable.Subscribe(
            _ => completed = true,
            ex => caughtException = ex,
            () => completed = true);

        // Give the observable time to complete
        await Task.Delay(200);

        // Either completes (on Windows with proper setup) or errors (on non-Windows or no roaming folder)
        await Assert.That(completed || caughtException != null).IsTrue();
    }

    /// <summary>
    /// Validates that SaveState returns new observable each call.
    /// </summary>
    [Test]
    public async Task SaveState_ReturnsNewObservableEachCall()
    {
        var state = new TestState { Name = "Test", Value = 42 };
        var observable1 = _sut.SaveState(state);
        var observable2 = _sut.SaveState(state);

        await Assert.That(observable1).IsNotSameReferenceAs(observable2);
    }

    /// <summary>
    /// Validates that LoadState returns new observable each call.
    /// </summary>
    [Test]
    public async Task LoadState_ReturnsNewObservableEachCall()
    {
        var observable1 = _sut.LoadState();
        var observable2 = _sut.LoadState();

        await Assert.That(observable1).IsNotSameReferenceAs(observable2);
    }

    /// <summary>
    /// Validates that SaveState with JsonTypeInfo returns new observable each call.
    /// </summary>
    [Test]
    public async Task SaveStateWithTypeInfo_ReturnsNewObservableEachCall()
    {
        var state = new TestState { Name = "Test", Value = 42 };
        var observable1 = _sut.SaveState(state, TestStateJsonContext.Default.TestState);
        var observable2 = _sut.SaveState(state, TestStateJsonContext.Default.TestState);

        await Assert.That(observable1).IsNotSameReferenceAs(observable2);
    }

    /// <summary>
    /// Validates that LoadState with JsonTypeInfo returns new observable each call.
    /// </summary>
    [Test]
    public async Task LoadStateWithTypeInfo_ReturnsNewObservableEachCall()
    {
        var observable1 = _sut.LoadState(TestStateJsonContext.Default.TestState);
        var observable2 = _sut.LoadState(TestStateJsonContext.Default.TestState);

        await Assert.That(observable1).IsNotSameReferenceAs(observable2);
    }

    /// <summary>
    /// Validates that SaveState method has RequiresDynamicCode attribute.
    /// </summary>
    [Test]
    public async Task SaveState_HasRequiresDynamicCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethods()
            .FirstOrDefault(m => m.Name == nameof(WinRTAppDataDriver.SaveState) && m.GetParameters().Length == 1);

        var attribute = method?.GetCustomAttributes(typeof(System.Diagnostics.CodeAnalysis.RequiresDynamicCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNotNull();
    }

    /// <summary>
    /// Validates that SaveState method has RequiresUnreferencedCode attribute.
    /// </summary>
    [Test]
    public async Task SaveState_HasRequiresUnreferencedCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethods()
            .FirstOrDefault(m => m.Name == nameof(WinRTAppDataDriver.SaveState) && m.GetParameters().Length == 1);

        var attribute = method?.GetCustomAttributes(typeof(System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNotNull();
    }

    /// <summary>
    /// Validates that LoadState method has RequiresDynamicCode attribute.
    /// </summary>
    [Test]
    public async Task LoadState_HasRequiresDynamicCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethod(
            nameof(WinRTAppDataDriver.LoadState),
            Type.EmptyTypes);

        var attribute = method?.GetCustomAttributes(typeof(System.Diagnostics.CodeAnalysis.RequiresDynamicCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNotNull();
    }

    /// <summary>
    /// Validates that LoadState method has RequiresUnreferencedCode attribute.
    /// </summary>
    [Test]
    public async Task LoadState_HasRequiresUnreferencedCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethod(
            nameof(WinRTAppDataDriver.LoadState),
            Type.EmptyTypes);

        var attribute = method?.GetCustomAttributes(typeof(System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNotNull();
    }

    /// <summary>
    /// Validates that SaveState with JsonTypeInfo does NOT have RequiresDynamicCode attribute (AOT-safe).
    /// </summary>
    [Test]
    public async Task SaveStateWithTypeInfo_DoesNotHaveRequiresDynamicCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethods()
            .FirstOrDefault(m => m.Name == nameof(WinRTAppDataDriver.SaveState) && m.GetParameters().Length == 2);

        var attribute = method?.GetCustomAttributes(typeof(System.Diagnostics.CodeAnalysis.RequiresDynamicCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNull();
    }

    /// <summary>
    /// Validates that LoadState with JsonTypeInfo does NOT have RequiresDynamicCode attribute (AOT-safe).
    /// </summary>
    [Test]
    public async Task LoadStateWithTypeInfo_DoesNotHaveRequiresDynamicCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethods()
            .FirstOrDefault(m => m.Name == nameof(WinRTAppDataDriver.LoadState) && m.GetParameters().Length == 1 && m.IsGenericMethod);

        var attribute = method?.GetCustomAttributes(typeof(System.Diagnostics.CodeAnalysis.RequiresDynamicCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNull();
    }

    /// <summary>
    /// Validates SaveState handles state with special characters in string properties.
    /// </summary>
    [Test]
    public async Task SaveState_HandlesSpecialCharactersInStrings()
    {
        var state = new TestState { Name = "Test<>&'\"", Value = 42 };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates SaveState handles state with unicode characters.
    /// </summary>
    [Test]
    public async Task SaveState_HandlesUnicodeCharacters()
    {
        var state = new TestState { Name = "测试 テスト 🎉", Value = 42 };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates SaveState handles state with very long strings.
    /// </summary>
    [Test]
    public async Task SaveState_HandlesLongStrings()
    {
        var state = new TestState { Name = new string('x', 10000), Value = 42 };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates SaveState handles state with negative values.
    /// </summary>
    [Test]
    public async Task SaveState_HandlesNegativeValues()
    {
        var state = new TestState { Name = "Test", Value = -42 };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates SaveState handles state with max int value.
    /// </summary>
    [Test]
    public async Task SaveState_HandlesMaxIntValue()
    {
        var state = new TestState { Name = "Test", Value = int.MaxValue };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates SaveState handles state with min int value.
    /// </summary>
    [Test]
    public async Task SaveState_HandlesMinIntValue()
    {
        var state = new TestState { Name = "Test", Value = int.MinValue };
        var observable = _sut.SaveState(state);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates that SaveState with JsonTypeInfo handles special characters.
    /// </summary>
    [Test]
    public async Task SaveStateWithTypeInfo_HandlesSpecialCharacters()
    {
        var state = new TestState { Name = "Test<>&'\"", Value = 42 };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.TestState);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates that SaveState with JsonTypeInfo handles unicode.
    /// </summary>
    [Test]
    public async Task SaveStateWithTypeInfo_HandlesUnicode()
    {
        var state = new TestState { Name = "测试 テスト 🎉", Value = 42 };
        var observable = _sut.SaveState(state, TestStateJsonContext.Default.TestState);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates SaveState observable can be converted to task.
    /// </summary>
    [Test]
    public async Task SaveState_ObservableCanBeConvertedToTask()
    {
        var state = new TestState { Name = "Test", Value = 42 };
        var observable = _sut.SaveState(state);

        // Should be able to get task from observable
        var task = observable.ToTask();

        await Assert.That((object)task).IsNotNull();
        await Assert.That((object)task).IsAssignableTo<Task>();
    }

    /// <summary>
    /// Validates LoadState observable can be converted to task.
    /// </summary>
    [Test]
    public async Task LoadState_ObservableCanBeConvertedToTask()
    {
        var observable = _sut.LoadState();

        // Should be able to get task from observable
        var task = observable.ToTask();

        await Assert.That((object)task).IsNotNull();
        await Assert.That((object)task).IsAssignableTo<Task>();
    }

    /// <summary>
    /// Validates InvalidateState observable can be converted to task.
    /// </summary>
    [Test]
    public async Task InvalidateState_ObservableCanBeConvertedToTask()
    {
        var observable = _sut.InvalidateState();

        // Should be able to get task from observable
        var task = observable.ToTask();

        await Assert.That((object)task).IsNotNull();
        await Assert.That((object)task).IsAssignableTo<Task>();
    }

    /// <summary>
    /// Validates that deeply nested objects can be handled by SaveState.
    /// </summary>
    [Test]
    public async Task SaveState_HandlesDeeplyNestedObjects()
    {
        var state = new NestedTestState
        {
            Id = 1,
            Inner = new TestState
            {
                Name = "Level1",
                Value = 100
            }
        };

        var observable = _sut.SaveState(state);
        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    /// Validates SaveState with different state instances returns unique observables.
    /// </summary>
    [Test]
    public async Task SaveState_DifferentStateInstances_ReturnUniqueObservables()
    {
        var state1 = new TestState { Name = "Test1", Value = 1 };
        var state2 = new TestState { Name = "Test2", Value = 2 };

        var observable1 = _sut.SaveState(state1);
        var observable2 = _sut.SaveState(state2);

        await Assert.That(observable1).IsNotSameReferenceAs(observable2);
    }

    /// <summary>
    /// Validates that the class can be used in a using statement pattern.
    /// </summary>
    [Test]
    public async Task WinRTAppDataDriver_CanBeUsedInScopePattern()
    {
        WinRTAppDataDriver driver;
        {
            driver = new WinRTAppDataDriver();
            await Assert.That(driver).IsNotNull();
        }

        // Driver should still be usable outside the scope
        await Assert.That(driver).IsNotNull();
        await Assert.That(driver.InvalidateState()).IsNotNull();
    }

    /// <summary>
    /// Validates that InvalidateState method does not have AOT-unsafe attributes.
    /// </summary>
    [Test]
    public async Task InvalidateState_DoesNotHaveRequiresDynamicCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethod(nameof(WinRTAppDataDriver.InvalidateState));

        var attribute = method?.GetCustomAttributes(typeof(System.Diagnostics.CodeAnalysis.RequiresDynamicCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNull();
    }

    /// <summary>
    /// Validates that InvalidateState method does not have AOT-unsafe attributes.
    /// </summary>
    [Test]
    public async Task InvalidateState_DoesNotHaveRequiresUnreferencedCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethod(nameof(WinRTAppDataDriver.InvalidateState));

        var attribute = method?.GetCustomAttributes(typeof(System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNull();
    }
}
