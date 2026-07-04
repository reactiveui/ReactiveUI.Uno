// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if !REACTIVE_SHIM
using ReactiveUI.Primitives.Concurrency;
#endif

namespace ReactiveUI.Uno.Tests.Schedulers;

/// <summary>Tests for <see cref="DeferredScheduler"/>.</summary>
public class DeferredSchedulerTests
{
    /// <summary>Verifies that Now is read from the scheduler returned by the factory.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Now_UsesFactoryScheduler()
    {
        var now = new DateTimeOffset(2026, 7, 4, 12, 0, 0, TimeSpan.Zero);
        var scheduler = new RecordingSequencer { Now = now };
        var sut = new DeferredScheduler(() => scheduler);

        await Assert.That(sut.Now).IsEqualTo(now);
    }

#if !REACTIVE_SHIM
    /// <summary>Verifies that Timestamp is read from the scheduler returned by the factory.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Timestamp_UsesFactoryScheduler()
    {
        const long timestamp = 1234;
        var scheduler = new RecordingSequencer { Timestamp = timestamp };
        var sut = new DeferredScheduler(() => scheduler);

        await Assert.That(sut.Timestamp).IsEqualTo(timestamp);
    }

    /// <summary>Verifies that immediate scheduling is delegated to the scheduler returned by the factory.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Schedule_DelegatesToFactoryScheduler()
    {
        var scheduler = new RecordingSequencer();
        var sut = new DeferredScheduler(() => scheduler);
        var item = new RecordingWorkItem();

        sut.Schedule(item);

        await Assert.That(scheduler.ScheduledItem).IsSameReferenceAs(item);
    }

    /// <summary>Verifies that delayed scheduling is delegated to the scheduler returned by the factory.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Schedule_WithDueTimestamp_DelegatesToFactoryScheduler()
    {
        const long dueTimestamp = 4567;
        var scheduler = new RecordingSequencer();
        var sut = new DeferredScheduler(() => scheduler);
        var item = new RecordingWorkItem();

        sut.Schedule(item, dueTimestamp);

        await Assert.That(scheduler.DelayedItem).IsSameReferenceAs(item);
        await Assert.That(scheduler.DueTimestamp).IsEqualTo(dueTimestamp);
    }

    /// <summary>Verifies that the extension overloads schedule through the native work item path.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Schedule_ExtensionOverload_DelegatesToNativeSchedule()
    {
        var scheduler = new RecordingSequencer();
        var sut = new DeferredScheduler(() => scheduler);

        using var disposable = sut.Schedule("state", static (_, _) => Disposable.Empty);

        await Assert.That(disposable).IsNotNull();
        await Assert.That(scheduler.ScheduledItem).IsNotNull();
    }
#else
    /// <summary>Verifies that immediate scheduling is delegated to the scheduler returned by the factory.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Schedule_DelegatesToFactoryScheduler()
    {
        var scheduler = new RecordingSequencer();
        var sut = new DeferredScheduler(() => scheduler);

        using var disposable = sut.Schedule("state", static (_, _) => Disposable.Empty);

        await Assert.That(disposable).IsNotNull();
        await Assert.That(scheduler.ScheduledState).IsEqualTo("state");
    }

    /// <summary>Verifies that delayed TimeSpan scheduling is delegated to the scheduler returned by the factory.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Schedule_WithDueTime_DelegatesToFactoryScheduler()
    {
        var dueTime = TimeSpan.FromSeconds(1);
        var scheduler = new RecordingSequencer();
        var sut = new DeferredScheduler(() => scheduler);

        using var disposable = sut.Schedule("state", dueTime, static (_, _) => Disposable.Empty);

        await Assert.That(disposable).IsNotNull();
        await Assert.That(scheduler.ScheduledState).IsEqualTo("state");
        await Assert.That(scheduler.DueTime).IsEqualTo(dueTime);
    }

    /// <summary>Verifies that delayed DateTimeOffset scheduling is delegated to the scheduler returned by the factory.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Schedule_WithDueDateTime_DelegatesToFactoryScheduler()
    {
        var dueTime = DateTimeOffset.UtcNow.AddSeconds(1);
        var scheduler = new RecordingSequencer();
        var sut = new DeferredScheduler(() => scheduler);

        using var disposable = sut.Schedule("state", dueTime, static (_, _) => Disposable.Empty);

        await Assert.That(disposable).IsNotNull();
        await Assert.That(scheduler.ScheduledState).IsEqualTo("state");
        await Assert.That(scheduler.DueDateTime).IsEqualTo(dueTime);
    }
#endif

    /// <summary>Verifies that all Schedule overloads validate the action delegate.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Schedule_ThrowsArgumentNullException_WhenActionIsNull()
    {
        var scheduler = new RecordingSequencer();
        var sut = new DeferredScheduler(() => scheduler);

        await Assert.That(() => sut.Schedule("state", null!)).Throws<ArgumentNullException>();
        await Assert.That(() => sut.Schedule("state", DateTimeOffset.Now, null!)).Throws<ArgumentNullException>();
        await Assert.That(() => sut.Schedule("state", TimeSpan.Zero, null!)).Throws<ArgumentNullException>();
    }

#if REACTIVE_SHIM
    /// <summary>Records scheduled work for deferred scheduler tests.</summary>
    private sealed class RecordingSequencer : ISequencer
    {
        /// <inheritdoc/>
        public DateTimeOffset Now { get; init; } = DateTimeOffset.UtcNow;

        /// <summary>Gets the most recent scheduled state.</summary>
        public object? ScheduledState { get; private set; }

        /// <summary>Gets the most recent relative due time.</summary>
        public TimeSpan? DueTime { get; private set; }

        /// <summary>Gets the most recent absolute due time.</summary>
        public DateTimeOffset? DueDateTime { get; private set; }

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            ScheduledState = state;
            return action(this, state);
        }

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            ScheduledState = state;
            DueTime = dueTime;
            return action(this, state);
        }

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            ScheduledState = state;
            DueDateTime = dueTime;
            return action(this, state);
        }
    }
#else
    /// <summary>Records scheduled work for deferred scheduler tests.</summary>
    private sealed class RecordingSequencer : ISequencer
    {
        /// <inheritdoc/>
        public DateTimeOffset Now { get; init; } = DateTimeOffset.UtcNow;

        /// <inheritdoc/>
        public long Timestamp { get; init; }

        /// <summary>Gets the most recent immediate work item.</summary>
        public IWorkItem? ScheduledItem { get; private set; }

        /// <summary>Gets the most recent delayed work item.</summary>
        public IWorkItem? DelayedItem { get; private set; }

        /// <summary>Gets the most recent delayed due timestamp.</summary>
        public long? DueTimestamp { get; private set; }

        /// <inheritdoc/>
        public void Schedule(IWorkItem item) => ScheduledItem = item;

        /// <inheritdoc/>
        public void Schedule(IWorkItem item, long dueTimestamp)
        {
            DelayedItem = item;
            DueTimestamp = dueTimestamp;
        }
    }

    /// <summary>Minimal work item used to verify scheduler delegation.</summary>
    private sealed class RecordingWorkItem : IWorkItem
    {
        /// <inheritdoc/>
        public void Execute()
        {
        }
    }
#endif
}
