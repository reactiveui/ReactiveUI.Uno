// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
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

/// <summary>Contains tests for the <see cref="ReactiveHelpers"/> class, ensuring its functionality for creating property observables and wiring activation.</summary>
public class ReactiveHelpersTests
{
    /// <summary>Validates that CreatePropertyChangedPulse throws ArgumentNullException when source is null.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyChangedPulse_ThrowsArgumentNullException_WhenSourceIsNull()
    {
        await Assert.That(() => ReactiveHelpers.CreatePropertyChangedPulse(null!, "TestProperty"))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Validates that CreatePropertyChangedPulse throws ArgumentNullException when propertyName is null.</summary>
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

        using var subscription = observable.Subscribe(value => emittedValues.Add(value));

        source.Name = "NewValue";

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

        using var subscription = observable.Subscribe(value => emittedValues.Add(value));

        source.Value = 42;

        await Assert.That(emittedValues.Count).IsZero();
    }

    /// <summary>Validates that CreatePropertyChangedPulse emits when PropertyName is null or empty (all properties changed).</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyChangedPulse_Emits_WhenPropertyNameIsNullOrEmpty()
    {
        var source = new TestNotifyPropertyChanged();
        var emittedValues = new List<Unit>();
        var observable = ReactiveHelpers.CreatePropertyChangedPulse(source, nameof(TestNotifyPropertyChanged.Name));

        using var subscription = observable.Subscribe(value => emittedValues.Add(value));

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

        var subscription = observable.Subscribe(value => emittedValues.Add(value));

        source.Name = "FirstChange";
        await Assert.That(emittedValues.Count).IsEqualTo(1);

        subscription.Dispose();

        source.Name = "SecondChange";
        await Assert.That(emittedValues.Count).IsEqualTo(1);
    }

    /// <summary>Validates that CreatePropertyValueObservable throws ArgumentNullException when source is null.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyValueObservable_ThrowsArgumentNullException_WhenSourceIsNull()
    {
        await Assert.That(() => ReactiveHelpers.CreatePropertyValueObservable<string>(null!, "TestProperty", () => "value"))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Validates that CreatePropertyValueObservable throws ArgumentNullException when propertyName is null.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyValueObservable_ThrowsArgumentNullException_WhenPropertyNameIsNull()
    {
        var source = new TestNotifyPropertyChanged();
        await Assert.That(() => ReactiveHelpers.CreatePropertyValueObservable<string>(source, null!, () => "value"))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Validates that CreatePropertyValueObservable throws ArgumentNullException when getPropertyValue is null.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyValueObservable_ThrowsArgumentNullException_WhenGetPropertyValueIsNull()
    {
        var source = new TestNotifyPropertyChanged();
        await Assert.That(() => ReactiveHelpers.CreatePropertyValueObservable<string>(source, "TestProperty", null!))
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
        var source = new TestNotifyPropertyChanged { Name = "InitialValue" };
        var emittedValues = new List<string?>();
        var observable = ReactiveHelpers.CreatePropertyValueObservable(
            source,
            nameof(TestNotifyPropertyChanged.Name),
            () => source.Name);

        using var subscription = observable.Subscribe(value => emittedValues.Add(value));

        await Assert.That(emittedValues.Count).IsEqualTo(1);
        await Assert.That(emittedValues[0]).IsEqualTo("InitialValue");
    }

    /// <summary>Validates that CreatePropertyValueObservable emits new value when property changes.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyValueObservable_EmitsNewValue_WhenPropertyChanges()
    {
        var source = new TestNotifyPropertyChanged { Name = "InitialValue" };
        var emittedValues = new List<string?>();
        var observable = ReactiveHelpers.CreatePropertyValueObservable(
            source,
            nameof(TestNotifyPropertyChanged.Name),
            () => source.Name);

        using var subscription = observable.Subscribe(value => emittedValues.Add(value));

        source.Name = "NewValue";

        await Assert.That(emittedValues.Count).IsEqualTo(2);
        await Assert.That(emittedValues[1]).IsEqualTo("NewValue");
    }

    /// <summary>Validates that CreatePropertyValueObservable does not emit when different property changes.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyValueObservable_DoesNotEmit_WhenDifferentPropertyChanges()
    {
        var source = new TestNotifyPropertyChanged { Name = "InitialValue" };
        var emittedValues = new List<string?>();
        var observable = ReactiveHelpers.CreatePropertyValueObservable(
            source,
            nameof(TestNotifyPropertyChanged.Name),
            () => source.Name);

        using var subscription = observable.Subscribe(value => emittedValues.Add(value));

        source.Value = 42;

        await Assert.That(emittedValues.Count).IsEqualTo(1);
    }

    /// <summary>Validates that CreatePropertyValueObservable stops emitting after disposal.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task CreatePropertyValueObservable_StopsEmitting_AfterDisposal()
    {
        var source = new TestNotifyPropertyChanged { Name = "InitialValue" };
        var emittedValues = new List<string?>();
        var observable = ReactiveHelpers.CreatePropertyValueObservable(
            source,
            nameof(TestNotifyPropertyChanged.Name),
            () => source.Name);

        var subscription = observable.Subscribe(value => emittedValues.Add(value));

        source.Name = "FirstChange";
        await Assert.That(emittedValues.Count).IsEqualTo(2);

        subscription.Dispose();

        source.Name = "SecondChange";
        await Assert.That(emittedValues.Count).IsEqualTo(2);
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
    /// Validates that WireActivationIfSupported returns Disposable.Empty when viewModel does not implement IActivatableViewModel.
    /// </summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task WireActivationIfSupported_ReturnsDisposableEmpty_WhenViewModelIsNotActivatable()
    {
        var viewModel = new NonActivatableViewModel();
        var activatedSignal = RxObservable.Never<Unit>();
        var deactivatedSignal = RxObservable.Never<Unit>();

        var result = ReactiveHelpers.WireActivationIfSupported(viewModel, activatedSignal, deactivatedSignal);

        await Assert.That(result).IsNotNull();
    }

    /// <summary>Validates that WireActivationIfSupported returns non-empty disposable when viewModel implements IActivatableViewModel.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task WireActivationIfSupported_ReturnsNonEmptyDisposable_WhenViewModelIsActivatable()
    {
        var viewModel = new ActivatableViewModel();
        var activatedSignal = RxObservable.Never<Unit>();
        var deactivatedSignal = RxObservable.Never<Unit>();

        var result = ReactiveHelpers.WireActivationIfSupported(viewModel, activatedSignal, deactivatedSignal);

        await Assert.That(result).IsNotNull();
    }

    /// <summary>Validates that WireActivationIfSupported forwards activation signals to an activatable view model.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task WireActivationIfSupported_ForwardsActivationSignals_ToViewModelActivator()
    {
        var viewModel = new ActivatableViewModel();
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
        var viewModel = new ActivatableViewModel();
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
        public void RaiseAllPropertiesChanged() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));

        /// <summary>Raises a property changed notification for the supplied property.</summary>
        /// <param name="propertyName">The changed property name.</param>
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>Non-activatable view model for testing.</summary>
    private sealed class NonActivatableViewModel
    {
        /// <summary>Gets a value indicating whether the test view model was initialized.</summary>
        public static bool IsInitialized => true;
    }

    /// <summary>Activatable view model for testing.</summary>
    private sealed class ActivatableViewModel : IActivatableViewModel
    {
        /// <inheritdoc/>
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
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
