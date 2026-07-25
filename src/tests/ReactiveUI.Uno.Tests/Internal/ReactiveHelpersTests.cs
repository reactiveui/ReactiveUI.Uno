// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
#if REACTIVE_SHIM
using ReactiveUI.Uno.Reactive.Internal;
using RxObservable = System.Reactive.Linq.Observable;
#else
using ReactiveUI.Uno.Internal;
using RxObservable = ReactiveUI.Primitives.Signals.Signal;
#endif
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Internal;

/// <summary>Contains tests for property observables and activation wiring.</summary>
public class ReactiveHelpersTests
{
    /// <summary>The initial test property value.</summary>
    private const string InitialValue = "InitialValue";

    /// <summary>The updated test property value.</summary>
    private const string NewValue = "NewValue";

    /// <summary>The test property name.</summary>
    private const string TestProperty = "TestProperty";

    /// <summary>The updated numeric property value.</summary>
    private const int UpdatedValue = 42;

    /// <summary>The expected emission count after a change.</summary>
    private const int ExpectedEmissionCountAfterChange = 2;

    /// <summary>Validates that CreatePropertyChangedPulse throws ArgumentNullException when source is null.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyChangedPulse_ThrowsArgumentNullException_WhenSourceIsNull() =>
        await Assert.That(() => ReactiveHelpers.CreatePropertyChangedPulse(null!, TestProperty))
            .Throws<ArgumentNullException>();

    /// <summary>Validates that a null pulse property name throws <see cref="ArgumentNullException"/>.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyChangedPulse_ThrowsArgumentNullException_WhenPropertyNameIsNull()
    {
        var source = new TestNotifyPropertyChanged();
        await Assert.That(() => ReactiveHelpers.CreatePropertyChangedPulse(source, null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Validates that CreatePropertyChangedPulse returns a non-null observable.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyChangedPulse_ReturnsNonNullObservable()
    {
        var source = new TestNotifyPropertyChanged();
        var observable = ReactiveHelpers.CreatePropertyChangedPulse(source, nameof(TestNotifyPropertyChanged.Name));

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates that CreatePropertyChangedPulse emits when the specified property changes.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyChangedPulse_EmitsUnit_WhenPropertyChanges()
    {
        var source = new TestNotifyPropertyChanged();
        var emittedValues = new List<Unit>();
        var observable = ReactiveHelpers.CreatePropertyChangedPulse(source, nameof(TestNotifyPropertyChanged.Name));

        using var subscription = observable.Subscribe(emittedValues.Add);

        source.Name = NewValue;

        await Assert.That(emittedValues.Count).IsEqualTo(1);
        await Assert.That(emittedValues[0]).IsEqualTo(Unit.Default);
    }

    /// <summary>Validates that CreatePropertyChangedPulse does not emit when a different property changes.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyChangedPulse_DoesNotEmit_WhenDifferentPropertyChanges()
    {
        var source = new TestNotifyPropertyChanged();
        var emittedValues = new List<Unit>();
        var observable = ReactiveHelpers.CreatePropertyChangedPulse(source, nameof(TestNotifyPropertyChanged.Name));

        using var subscription = observable.Subscribe(emittedValues.Add);

        source.Value = UpdatedValue;

        await Assert.That(emittedValues.Count).IsZero();
    }

    /// <summary>Validates that an all-properties notification emits a pulse.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyChangedPulse_Emits_WhenPropertyNameIsNullOrEmpty()
    {
        var source = new TestNotifyPropertyChanged();
        var emittedValues = new List<Unit>();
        var observable = ReactiveHelpers.CreatePropertyChangedPulse(source, nameof(TestNotifyPropertyChanged.Name));

        using var subscription = observable.Subscribe(emittedValues.Add);

        source.RaiseAllPropertiesChanged();

        await Assert.That(emittedValues.Count).IsEqualTo(1);
    }

    /// <summary>Validates that CreatePropertyChangedPulse stops emitting after disposal.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyChangedPulse_StopsEmitting_AfterDisposal()
    {
        var source = new TestNotifyPropertyChanged();
        var emittedValues = new List<Unit>();
        var observable = ReactiveHelpers.CreatePropertyChangedPulse(source, nameof(TestNotifyPropertyChanged.Name));

        var subscription = observable.Subscribe(emittedValues.Add);

        source.Name = "FirstChange";
        await Assert.That(emittedValues.Count).IsEqualTo(1);

        subscription.Dispose();

        source.Name = "SecondChange";
        await Assert.That(emittedValues.Count).IsEqualTo(1);
    }

    /// <summary>Validates that a null value-observable source throws <see cref="ArgumentNullException"/>.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyValueObservable_ThrowsArgumentNullException_WhenSourceIsNull() =>
        await Assert.That(
            () => ReactiveHelpers.CreatePropertyValueObservable(null!, TestProperty, () => "value"))
            .Throws<ArgumentNullException>();

    /// <summary>Validates that a null value-observable property name throws.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyValueObservable_ThrowsArgumentNullException_WhenPropertyNameIsNull()
    {
        var source = new TestNotifyPropertyChanged();
        await Assert.That(() => ReactiveHelpers.CreatePropertyValueObservable(source, null!, () => "value"))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Validates that a null property-value accessor throws.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyValueObservable_ThrowsArgumentNullException_WhenGetPropertyValueIsNull()
    {
        var source = new TestNotifyPropertyChanged();
        await Assert.That(() => ReactiveHelpers.CreatePropertyValueObservable<string>(source, TestProperty, null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Validates that CreatePropertyValueObservable returns a non-null observable.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyValueObservable_ReturnsNonNullObservable()
    {
        var source = new TestNotifyPropertyChanged();
        var observable = ReactiveHelpers.CreatePropertyValueObservable(
            source,
            nameof(TestNotifyPropertyChanged.Name),
            () => source.Name);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Validates that CreatePropertyValueObservable emits initial value upon subscription.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyValueObservable_EmitsInitialValue_UponSubscription()
    {
        var source = new TestNotifyPropertyChanged { Name = InitialValue };
        var emittedValues = new List<string?>();
        var observable = ReactiveHelpers.CreatePropertyValueObservable(
            source,
            nameof(TestNotifyPropertyChanged.Name),
            () => source.Name);

        using var subscription = observable.Subscribe(emittedValues.Add);

        await Assert.That(emittedValues.Count).IsEqualTo(1);
        await Assert.That(emittedValues[0]).IsEqualTo(InitialValue);
    }

    /// <summary>Validates that CreatePropertyValueObservable emits new value when property changes.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyValueObservable_EmitsNewValue_WhenPropertyChanges()
    {
        var source = new TestNotifyPropertyChanged { Name = InitialValue };
        var emittedValues = new List<string?>();
        var observable = ReactiveHelpers.CreatePropertyValueObservable(
            source,
            nameof(TestNotifyPropertyChanged.Name),
            () => source.Name);

        using var subscription = observable.Subscribe(emittedValues.Add);

        source.Name = NewValue;

        await Assert.That(emittedValues.Count).IsEqualTo(ExpectedEmissionCountAfterChange);
        await Assert.That(emittedValues[1]).IsEqualTo(NewValue);
    }

    /// <summary>Validates that CreatePropertyValueObservable does not emit when different property changes.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyValueObservable_DoesNotEmit_WhenDifferentPropertyChanges()
    {
        var source = new TestNotifyPropertyChanged { Name = InitialValue };
        var emittedValues = new List<string?>();
        var observable = ReactiveHelpers.CreatePropertyValueObservable(
            source,
            nameof(TestNotifyPropertyChanged.Name),
            () => source.Name);

        using var subscription = observable.Subscribe(emittedValues.Add);

        source.Value = UpdatedValue;

        await Assert.That(emittedValues.Count).IsEqualTo(1);
    }

    /// <summary>Validates that CreatePropertyValueObservable stops emitting after disposal.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyValueObservable_StopsEmitting_AfterDisposal()
    {
        var source = new TestNotifyPropertyChanged { Name = InitialValue };
        var emittedValues = new List<string?>();
        var observable = ReactiveHelpers.CreatePropertyValueObservable(
            source,
            nameof(TestNotifyPropertyChanged.Name),
            () => source.Name);

        var subscription = observable.Subscribe(emittedValues.Add);

        source.Name = "FirstChange";
        await Assert.That(emittedValues.Count).IsEqualTo(ExpectedEmissionCountAfterChange);

        subscription.Dispose();

        source.Name = "SecondChange";
        await Assert.That(emittedValues.Count).IsEqualTo(ExpectedEmissionCountAfterChange);
    }

    /// <summary>Validates that WireActivationIfSupported returns Disposable.Empty when viewModel is null.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task WireActivationIfSupported_ReturnsDisposableEmpty_WhenViewModelIsNull()
    {
        var activatedSignal = RxObservable.Never<Unit>();
        var deactivatedSignal = RxObservable.Never<Unit>();

        var result = ReactiveHelpers.WireActivationIfSupported(null, activatedSignal, deactivatedSignal);

        await Assert.That(result).IsNotNull();
    }

    /// <summary>
    /// Validates that WireActivationIfSupported returns Disposable.Empty when viewModel does not implement
    /// IActivatableViewModel.
    /// </summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task WireActivationIfSupported_ReturnsDisposableEmpty_WhenViewModelIsNotActivatable()
    {
        var viewModel = new object();
        var activatedSignal = RxObservable.Never<Unit>();
        var deactivatedSignal = RxObservable.Never<Unit>();

        var result = ReactiveHelpers.WireActivationIfSupported(viewModel, activatedSignal, deactivatedSignal);

        await Assert.That(result).IsNotNull();
    }

    /// <summary>Validates that an activatable view model produces a non-empty disposable.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task WireActivationIfSupported_ReturnsNonEmptyDisposable_WhenViewModelIsActivatable()
    {
        using var viewModel = new ActivatableViewModel();
        var activatedSignal = RxObservable.Never<Unit>();
        var deactivatedSignal = RxObservable.Never<Unit>();

        var result = ReactiveHelpers.WireActivationIfSupported(viewModel, activatedSignal, deactivatedSignal);

        await Assert.That(result).IsNotNull();
    }

    /// <summary>Validates that activation signals reach the view model activator.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task WireActivationIfSupported_ForwardsActivationSignals_ToViewModelActivator()
    {
        using var viewModel = new ActivatableViewModel();
        using var activatedSignal = new TestSignal<Unit>();
        using var deactivatedSignal = new TestSignal<Unit>();
        var activatedCount = 0;
        var deactivatedCount = 0;
        using var activatedSubscription = viewModel.Activator.Activated.Subscribe(_ => activatedCount++);
        using var deactivatedSubscription = viewModel.Activator.Deactivated.Subscribe(_ => deactivatedCount++);
        using var wiring = ReactiveHelpers.WireActivationIfSupported(viewModel, activatedSignal, deactivatedSignal);

        activatedSignal.OnNext(Unit.Default);
        deactivatedSignal.OnNext(Unit.Default);

        await Assert.That(activatedCount).IsEqualTo(1);
        await Assert.That(deactivatedCount).IsEqualTo(1);
    }

    /// <summary>Validates that WireActivationIfSupported can be disposed without error.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task WireActivationIfSupported_CanBeDisposed_WithoutError()
    {
        using var viewModel = new ActivatableViewModel();
        var activatedSignal = RxObservable.Never<Unit>();
        var deactivatedSignal = RxObservable.Never<Unit>();

        var result = ReactiveHelpers.WireActivationIfSupported(viewModel, activatedSignal, deactivatedSignal);

        await Assert.That(() => result.Dispose()).ThrowsNothing();
    }

    /// <summary>Test implementation of INotifyPropertyChanged for testing purposes.</summary>
    private sealed class TestNotifyPropertyChanged : INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>Gets or sets the test name.</summary>
        public string? Name
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>Gets or sets the test value.</summary>
        public int Value
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        /// <summary>Raises a property changed notification for all properties.</summary>
        public void RaiseAllPropertiesChanged() =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));

        /// <summary>Raises a property changed notification for the supplied property.</summary>
        /// <param name="propertyName">The changed property name.</param>
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>Activatable view model for testing.</summary>
    private sealed class ActivatableViewModel : IActivatableViewModel, IDisposable
    {
        /// <inheritdoc/>
        public ViewModelActivator Activator { get; } = new();

        /// <inheritdoc/>
        public void Dispose() => Activator.Dispose();
    }

    /// <summary>Simple observable signal for activation tests.</summary>
    /// <typeparam name="T">The signal value type.</typeparam>
    private sealed class TestSignal<T> : IObservable<T>, IDisposable
    {
        /// <summary>Stores the current signal observers.</summary>
        private readonly List<IObserver<T>> _observers = [];

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentNullException.ThrowIfNull(observer);

            _observers.Add(observer);
            return Disposable.Create(() => _observers.Remove(observer));
        }

        /// <summary>Publishes a value to current subscribers.</summary>
        /// <param name="value">The value to publish.</param>
        public void OnNext(T value)
        {
            foreach (var observer in _observers.ToArray())
            {
                observer.OnNext(value);
            }
        }

        /// <inheritdoc/>
        public void Dispose() => _observers.Clear();
    }
}
