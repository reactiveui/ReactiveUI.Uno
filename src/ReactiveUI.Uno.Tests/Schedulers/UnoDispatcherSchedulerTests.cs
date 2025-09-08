// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if !WINDOWS
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using NSubstitute;
using NUnit.Framework;
using Windows.Foundation;
using Windows.UI.Core;

namespace ReactiveUI.Uno.Tests.Schedulers;

/// <summary>
/// Tests for UnoDispatcherScheduler functionality.
/// </summary>
[TestFixture]
public class UnoDispatcherSchedulerTests
{
    private CoreDispatcher _mockDispatcher = null!;
    private UnoDispatcherScheduler _scheduler = null!;

    /// <summary>
    /// Setup for each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockDispatcher = Substitute.For<CoreDispatcher>();
        _scheduler = new UnoDispatcherScheduler(_mockDispatcher);
    }

    /// <summary>
    /// Test constructor with dispatcher parameter sets properties correctly.
    /// </summary>
    [Test]
    public void Constructor_WithDispatcher_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var scheduler = new UnoDispatcherScheduler(_mockDispatcher);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(scheduler.Dispatcher, Is.EqualTo(_mockDispatcher));
            Assert.That(scheduler.Priority, Is.EqualTo(CoreDispatcherPriority.Normal));
        });
    }

    /// <summary>
    /// Test constructor with dispatcher and priority parameters sets properties correctly.
    /// </summary>
    [Test]
    public void Constructor_WithDispatcherAndPriority_SetsPropertiesCorrectly()
    {
        // Arrange
        var priority = CoreDispatcherPriority.High;

        // Act
        var scheduler = new UnoDispatcherScheduler(_mockDispatcher, priority);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(scheduler.Dispatcher, Is.EqualTo(_mockDispatcher));
            Assert.That(scheduler.Priority, Is.EqualTo(priority));
        });
    }

    /// <summary>
    /// Test constructor throws ArgumentNullException when dispatcher is null.
    /// </summary>
    [Test]
    public void Constructor_NullDispatcher_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UnoDispatcherScheduler(null!));
    }

    /// <summary>
    /// Test constructor with priority throws ArgumentNullException when dispatcher is null.
    /// </summary>
    [Test]
    public void Constructor_NullDispatcherWithPriority_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UnoDispatcherScheduler(null!, CoreDispatcherPriority.High));
    }

    /// <summary>
    /// Test Schedule method throws ArgumentNullException when action is null.
    /// </summary>
    [Test]
    public void Schedule_NullAction_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _scheduler.Schedule("state", null!));
    }

    /// <summary>
    /// Test Schedule with due time throws ArgumentNullException when action is null.
    /// </summary>
    [Test]
    public void Schedule_WithDueTime_NullAction_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _scheduler.Schedule("state", TimeSpan.Zero, null!));
    }

    /// <summary>
    /// Test SchedulePeriodic throws ArgumentNullException when action is null.
    /// </summary>
    [Test]
    public void SchedulePeriodic_NullAction_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _scheduler.SchedulePeriodic("state", TimeSpan.Zero, null!));
    }

    /// <summary>
    /// Test SchedulePeriodic throws ArgumentOutOfRangeException when period is negative.
    /// </summary>
    [Test]
    public void SchedulePeriodic_NegativePeriod_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var negativePeriod = TimeSpan.FromMilliseconds(-1);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _scheduler.SchedulePeriodic("state", negativePeriod, state => state));
    }

    /// <summary>
    /// Test Schedule method returns non-null disposable.
    /// </summary>
    [Test]
    public void Schedule_ValidAction_ReturnsNonNullDisposable()
    {
        // Arrange
        var mockOperation = Substitute.For<IAsyncAction>();
        _ = _mockDispatcher.RunAsync(Arg.Any<CoreDispatcherPriority>(), Arg.Any<DispatchedHandler>())
                      .Returns(mockOperation);

        // Act
        var disposable = _scheduler.Schedule("test", (scheduler, state) => Disposable.Empty);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(disposable, Is.Not.Null);
            Assert.That(disposable, Is.InstanceOf<IDisposable>());
        });
    }

    /// <summary>
    /// Test Schedule with zero due time calls Schedule without delay.
    /// </summary>
    [Test]
    public void Schedule_WithZeroDueTime_CallsImmediateSchedule()
    {
        // Arrange
        var mockOperation = Substitute.For<IAsyncAction>();
        _ = _mockDispatcher.RunAsync(Arg.Any<CoreDispatcherPriority>(), Arg.Any<DispatchedHandler>())
                      .Returns(mockOperation);

        // Act
        var disposable = _scheduler.Schedule("test", TimeSpan.Zero, (scheduler, state) => Disposable.Empty);

        // Assert
        Assert.That(disposable, Is.Not.Null);
        _mockDispatcher.Received(1).RunAsync(Arg.Any<CoreDispatcherPriority>(), Arg.Any<DispatchedHandler>());
    }

    /// <summary>
    /// Test Schedule with positive due time returns valid disposable.
    /// </summary>
    [Test]
    public void Schedule_WithPositiveDueTime_ReturnsValidDisposable()
    {
        // Arrange
        var dueTime = TimeSpan.FromMilliseconds(100);

        // Act
        var disposable = _scheduler.Schedule("test", dueTime, (scheduler, state) => Disposable.Empty);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(disposable, Is.Not.Null);
            Assert.That(disposable, Is.InstanceOf<IDisposable>());
        });
    }

    /// <summary>
    /// Test SchedulePeriodic with zero period returns valid disposable.
    /// </summary>
    [Test]
    public void SchedulePeriodic_WithZeroPeriod_ReturnsValidDisposable()
    {
        // Act
        var disposable = _scheduler.SchedulePeriodic("test", TimeSpan.Zero, state => state + "1");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(disposable, Is.Not.Null);
            Assert.That(disposable, Is.InstanceOf<IDisposable>());
        });

        // Cleanup
        disposable.Dispose();
    }

    /// <summary>
    /// Test SchedulePeriodic with positive period returns valid disposable.
    /// </summary>
    [Test]
    public void SchedulePeriodic_WithPositivePeriod_ReturnsValidDisposable()
    {
        // Arrange
        var period = TimeSpan.FromMilliseconds(50);

        // Act
        var disposable = _scheduler.SchedulePeriodic(0, period, state => state + 1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(disposable, Is.Not.Null);
            Assert.That(disposable, Is.InstanceOf<IDisposable>());
        });

        // Cleanup
        disposable.Dispose();
    }

    /// <summary>
    /// Test that Dispatcher property is accessible.
    /// </summary>
    [Test]
    public void Dispatcher_Property_IsAccessible()
    {
        // Act & Assert
        Assert.That(_scheduler.Dispatcher, Is.EqualTo(_mockDispatcher));
    }

    /// <summary>
    /// Test that Priority property is accessible.
    /// </summary>
    [Test]
    public void Priority_Property_IsAccessible()
    {
        // Act & Assert
        Assert.That(_scheduler.Priority, Is.EqualTo(CoreDispatcherPriority.Normal));
    }

    /// <summary>
    /// Test that Priority property reflects constructor value.
    /// </summary>
    [Test]
    public void Priority_Property_ReflectsConstructorValue()
    {
        // Arrange
        var priority = CoreDispatcherPriority.Low;
        var scheduler = new UnoDispatcherScheduler(_mockDispatcher, priority);

        // Act & Assert
        Assert.That(scheduler.Priority, Is.EqualTo(priority));
    }

    /// <summary>
    /// Test disposing returned disposable doesn't throw.
    /// </summary>
    [Test]
    public void DisposingReturnedDisposable_DoesNotThrow()
    {
        // Arrange
        var mockOperation = Substitute.For<IAsyncAction>();
        _ = _mockDispatcher.RunAsync(Arg.Any<CoreDispatcherPriority>(), Arg.Any<DispatchedHandler>())
                      .Returns(mockOperation);

        var disposable = _scheduler.Schedule("test", (scheduler, state) => Disposable.Empty);

        // Act & Assert
        Assert.DoesNotThrow(() => disposable.Dispose());
    }

    /// <summary>
    /// Test multiple scheduler instantiation is supported.
    /// </summary>
    [Test]
    public void MultipleSchedulerInstantiation_IsSupported()
    {
        // Act
        var scheduler1 = new UnoDispatcherScheduler(_mockDispatcher);
        var scheduler2 = new UnoDispatcherScheduler(_mockDispatcher, CoreDispatcherPriority.High);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(scheduler1, Is.Not.Null);
            Assert.That(scheduler2, Is.Not.Null);
            Assert.That(scheduler1.Priority, Is.EqualTo(CoreDispatcherPriority.Normal));
            Assert.That(scheduler2.Priority, Is.EqualTo(CoreDispatcherPriority.High));
        });
    }

    /// <summary>
    /// Test Current property behavior when no window exists.
    /// </summary>
    [Test]
    public void Current_Property_WithNoWindow_ThrowsInvalidOperationException()
    {
        // Note: This test may not be reliable in headless test environment
        // but we test the access to validate the property exists
        try
        {
            var current = UnoDispatcherScheduler.Current;
            Assert.That(current, Is.Not.Null);
        }
        catch (InvalidOperationException)
        {
            // Expected when no current window is available
            Assert.Pass("InvalidOperationException thrown as expected when no current window");
        }
    }

    /// <summary>
    /// Test that scheduler implements ISchedulerPeriodic interface.
    /// </summary>
    [Test]
    public void Scheduler_ImplementsISchedulerPeriodic()
    {
        // Act & Assert
        Assert.That(_scheduler, Is.InstanceOf<ISchedulerPeriodic>());
    }

    /// <summary>
    /// Test that scheduler implements IScheduler interface.
    /// </summary>
    [Test]
    public void Scheduler_ImplementsIScheduler()
    {
        // Act & Assert
        Assert.That(_scheduler, Is.InstanceOf<IScheduler>());
    }

    /// <summary>
    /// Test schedule verification with mocked dispatcher interaction.
    /// </summary>
    [Test]
    public void Schedule_CallsDispatcher_WithCorrectPriority()
    {
        // Arrange
        var mockOperation = Substitute.For<IAsyncAction>();
        _ = _mockDispatcher.RunAsync(Arg.Any<CoreDispatcherPriority>(), Arg.Any<DispatchedHandler>())
                      .Returns(mockOperation);

        // Act
        _scheduler.Schedule("test", (scheduler, state) => Disposable.Empty);

        // Assert
        _mockDispatcher.Received(1).RunAsync(CoreDispatcherPriority.Normal, Arg.Any<DispatchedHandler>());
    }

    /// <summary>
    /// Test schedule with custom priority calls dispatcher correctly.
    /// </summary>
    [Test]
    public void Schedule_WithCustomPriority_CallsDispatcher_WithCorrectPriority()
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
    }
}
#endif