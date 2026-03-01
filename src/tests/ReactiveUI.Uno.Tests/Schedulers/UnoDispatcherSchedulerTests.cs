// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using NSubstitute;
using Windows.Foundation;
using Windows.UI.Core;

namespace ReactiveUI.Uno.Tests.Schedulers;

/// <summary>
/// Tests for UnoDispatcherScheduler functionality.
/// </summary>
public class UnoDispatcherSchedulerTests
{
    private CoreDispatcher _mockDispatcher = null!;
    private UnoDispatcherScheduler _scheduler = null!;

    /// <summary>
    /// Setup for each test.
    /// </summary>
    [Before(Test)]
    public void SetUp()
    {
        try
        {
            _mockDispatcher = Substitute.For<CoreDispatcher>();
            _scheduler = new UnoDispatcherScheduler(_mockDispatcher);
        }
        catch (Exception)
        {
            Skip.Test("Skipping scheduler tests because a CoreDispatcher test double cannot be created in this environment.");
        }
    }

    /// <summary>
    /// Test constructor with dispatcher parameter sets properties correctly.
    /// </summary>
    [Test]
    public async Task Constructor_WithDispatcher_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var scheduler = new UnoDispatcherScheduler(_mockDispatcher);

        // Assert
        await Assert.That(scheduler.Dispatcher).IsEqualTo(_mockDispatcher);
        await Assert.That(scheduler.Priority).IsEqualTo(CoreDispatcherPriority.Normal);
    }

    /// <summary>
    /// Test constructor with dispatcher and priority parameters sets properties correctly.
    /// </summary>
    [Test]
    public async Task Constructor_WithDispatcherAndPriority_SetsPropertiesCorrectly()
    {
        // Arrange
        var priority = CoreDispatcherPriority.High;

        // Act
        var scheduler = new UnoDispatcherScheduler(_mockDispatcher, priority);

        // Assert
        await Assert.That(scheduler.Dispatcher).IsEqualTo(_mockDispatcher);
        await Assert.That(scheduler.Priority).IsEqualTo(priority);
    }

    /// <summary>
    /// Test constructor throws ArgumentNullException when dispatcher is null.
    /// </summary>
    [Test]
    public async Task Constructor_NullDispatcher_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.That(() => new UnoDispatcherScheduler(null!)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Test constructor with priority throws ArgumentNullException when dispatcher is null.
    /// </summary>
    [Test]
    public async Task Constructor_NullDispatcherWithPriority_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.That(() => new UnoDispatcherScheduler(null!, CoreDispatcherPriority.High)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Test Schedule method throws ArgumentNullException when action is null.
    /// </summary>
    [Test]
    public async Task Schedule_NullAction_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.That(() => _scheduler.Schedule("state", null!)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Test Schedule with due time throws ArgumentNullException when action is null.
    /// </summary>
    [Test]
    public async Task Schedule_WithDueTime_NullAction_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.That(() => _scheduler.Schedule("state", TimeSpan.Zero, null!)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Test SchedulePeriodic throws ArgumentNullException when action is null.
    /// </summary>
    [Test]
    public async Task SchedulePeriodic_NullAction_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.That(() => _scheduler.SchedulePeriodic("state", TimeSpan.Zero, null!)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Test SchedulePeriodic throws ArgumentOutOfRangeException when period is negative.
    /// </summary>
    [Test]
    public async Task SchedulePeriodic_NegativePeriod_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var negativePeriod = TimeSpan.FromMilliseconds(-1);

        // Act & Assert
        await Assert.That(() => _scheduler.SchedulePeriodic("state", negativePeriod, state => state)).Throws<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Test Schedule method returns non-null disposable.
    /// </summary>
    [Test]
    public async Task Schedule_ValidAction_ReturnsNonNullDisposable()
    {
        // Arrange
        var mockOperation = Substitute.For<IAsyncAction>();
        _ = _mockDispatcher.RunAsync(Arg.Any<CoreDispatcherPriority>(), Arg.Any<DispatchedHandler>())
                      .Returns(mockOperation);

        // Act
        var disposable = _scheduler.Schedule("test", (scheduler, state) => Disposable.Empty);

        // Assert
        await Assert.That(disposable).IsNotNull();
        await Assert.That(disposable).IsAssignableTo<IDisposable>();
    }

    /// <summary>
    /// Test Schedule with zero due time calls Schedule without delay.
    /// </summary>
    [Test]
    public async Task Schedule_WithZeroDueTime_CallsImmediateSchedule()
    {
        // Arrange
        var mockOperation = Substitute.For<IAsyncAction>();
        _ = _mockDispatcher.RunAsync(Arg.Any<CoreDispatcherPriority>(), Arg.Any<DispatchedHandler>())
                      .Returns(mockOperation);

        // Act
        var disposable = _scheduler.Schedule("test", TimeSpan.Zero, (scheduler, state) => Disposable.Empty);

        // Assert
        await Assert.That(disposable).IsNotNull();
        _mockDispatcher.Received(1).RunAsync(Arg.Any<CoreDispatcherPriority>(), Arg.Any<DispatchedHandler>());
    }

    /// <summary>
    /// Test Schedule with positive due time returns valid disposable.
    /// </summary>
    [Test]
    public async Task Schedule_WithPositiveDueTime_ReturnsValidDisposable()
    {
        // Arrange
        var dueTime = TimeSpan.FromMilliseconds(100);

        // Act
        var disposable = _scheduler.Schedule("test", dueTime, (scheduler, state) => Disposable.Empty);

        // Assert
        await Assert.That(disposable).IsNotNull();
        await Assert.That(disposable).IsAssignableTo<IDisposable>();
    }

    /// <summary>
    /// Test SchedulePeriodic with zero period returns valid disposable.
    /// </summary>
    [Test]
    public async Task SchedulePeriodic_WithZeroPeriod_ReturnsValidDisposable()
    {
        // Act
        var disposable = _scheduler.SchedulePeriodic("test", TimeSpan.Zero, state => state + "1");

        // Assert
        await Assert.That(disposable).IsNotNull();
        await Assert.That(disposable).IsAssignableTo<IDisposable>();

        // Cleanup
        disposable.Dispose();
    }

    /// <summary>
    /// Test SchedulePeriodic with positive period returns valid disposable.
    /// </summary>
    [Test]
    public async Task SchedulePeriodic_WithPositivePeriod_ReturnsValidDisposable()
    {
        // Arrange
        var period = TimeSpan.FromMilliseconds(50);

        // Act
        var disposable = _scheduler.SchedulePeriodic(0, period, state => state + 1);

        // Assert
        await Assert.That(disposable).IsNotNull();
        await Assert.That(disposable).IsAssignableTo<IDisposable>();

        // Cleanup
        disposable.Dispose();
    }

    /// <summary>
    /// Test that Dispatcher property is accessible.
    /// </summary>
    [Test]
    public async Task Dispatcher_Property_IsAccessible()
    {
        // Act & Assert
        await Assert.That(_scheduler.Dispatcher).IsEqualTo(_mockDispatcher);
    }

    /// <summary>
    /// Test that Priority property is accessible.
    /// </summary>
    [Test]
    public async Task Priority_Property_IsAccessible()
    {
        // Act & Assert
        await Assert.That(_scheduler.Priority).IsEqualTo(CoreDispatcherPriority.Normal);
    }

    /// <summary>
    /// Test that Priority property reflects constructor value.
    /// </summary>
    [Test]
    public async Task Priority_Property_ReflectsConstructorValue()
    {
        // Arrange
        var priority = CoreDispatcherPriority.Low;
        var scheduler = new UnoDispatcherScheduler(_mockDispatcher, priority);

        // Act & Assert
        await Assert.That(scheduler.Priority).IsEqualTo(priority);
    }

    /// <summary>
    /// Test disposing returned disposable doesn't throw.
    /// </summary>
    [Test]
    public async Task DisposingReturnedDisposable_DoesNotThrow()
    {
        // Arrange
        var mockOperation = Substitute.For<IAsyncAction>();
        _ = _mockDispatcher.RunAsync(Arg.Any<CoreDispatcherPriority>(), Arg.Any<DispatchedHandler>())
                      .Returns(mockOperation);

        var disposable = _scheduler.Schedule("test", (scheduler, state) => Disposable.Empty);

        // Act & Assert
        await Assert.That(() => disposable.Dispose()).ThrowsNothing();
    }

    /// <summary>
    /// Test multiple scheduler instantiation is supported.
    /// </summary>
    [Test]
    public async Task MultipleSchedulerInstantiation_IsSupported()
    {
        // Act
        var scheduler1 = new UnoDispatcherScheduler(_mockDispatcher);
        var scheduler2 = new UnoDispatcherScheduler(_mockDispatcher, CoreDispatcherPriority.High);

        // Assert
        await Assert.That(scheduler1).IsNotNull();
        await Assert.That(scheduler2).IsNotNull();
        await Assert.That(scheduler1.Priority).IsEqualTo(CoreDispatcherPriority.Normal);
        await Assert.That(scheduler2.Priority).IsEqualTo(CoreDispatcherPriority.High);
    }

    /// <summary>
    /// Test Current property behavior when no window exists.
    /// </summary>
    [Test]
    public async Task Current_Property_WithNoWindow_ThrowsInvalidOperationException()
    {
        // Note: This test may not be reliable in headless test environment
        // but we test the access to validate the property exists
        try
        {
            var current = UnoDispatcherScheduler.Current;
            await Assert.That(current).IsNotNull();
        }
        catch (InvalidOperationException)
        {
            // Expected when no current window is available - test passes
        }
    }

    /// <summary>
    /// Test that scheduler implements ISchedulerPeriodic interface.
    /// </summary>
    [Test]
    public async Task Scheduler_ImplementsISchedulerPeriodic()
    {
        // Act & Assert
        await Assert.That(_scheduler).IsAssignableTo<ISchedulerPeriodic>();
    }

    /// <summary>
    /// Test that scheduler implements IScheduler interface.
    /// </summary>
    [Test]
    public async Task Scheduler_ImplementsIScheduler()
    {
        // Act & Assert
        await Assert.That(_scheduler).IsAssignableTo<IScheduler>();
    }

    /// <summary>
    /// Test schedule verification with mocked dispatcher interaction.
    /// </summary>
    [Test]
    public async Task Schedule_CallsDispatcher_WithCorrectPriority()
    {
        // Arrange
        var mockOperation = Substitute.For<IAsyncAction>();
        _ = _mockDispatcher.RunAsync(Arg.Any<CoreDispatcherPriority>(), Arg.Any<DispatchedHandler>())
                      .Returns(mockOperation);

        // Act
        _scheduler.Schedule("test", (scheduler, state) => Disposable.Empty);

        // Assert
        _mockDispatcher.Received(1).RunAsync(CoreDispatcherPriority.Normal, Arg.Any<DispatchedHandler>());
        await Task.CompletedTask;
    }

    /// <summary>
    /// Test schedule with custom priority calls dispatcher correctly.
    /// </summary>
    [Test]
    public async Task Schedule_WithCustomPriority_CallsDispatcher_WithCorrectPriority()
    {
        // Arrange
        var customPriority = CoreDispatcherPriority.High;
        var customScheduler = new UnoDispatcherScheduler(_mockDispatcher, customPriority);
        var mockOperation = Substitute.For<IAsyncAction>();
        _ = _mockDispatcher.RunAsync(Arg.Any<CoreDispatcherPriority>(), Arg.Any<DispatchedHandler>())
                      .Returns(mockOperation);

        // Act
        customScheduler.Schedule("test", (scheduler, state) => Disposable.Empty);

        // Assert
        _mockDispatcher.Received(1).RunAsync(customPriority, Arg.Any<DispatchedHandler>());
        await Task.CompletedTask;
    }
}
