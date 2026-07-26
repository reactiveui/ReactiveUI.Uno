// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if !REACTIVE_SHIM
using ReactiveUI.Primitives.Concurrency;
#endif

#if REACTIVE_SHIM

namespace ReactiveUI.Uno.Reactive;
#else

namespace ReactiveUI.Uno;
#endif

/// <summary>Defers scheduler resolution until each scheduled operation is executed.</summary>
/// <param name="schedulerFactory">The factory used to resolve the current scheduler.</param>
internal sealed class DeferredScheduler(Func<IScheduler> schedulerFactory) : IScheduler
{
    /// <inheritdoc/>
    public DateTimeOffset Now => CurrentScheduler.Now;

#if !REACTIVE_SHIM
    /// <inheritdoc/>
    public long Timestamp => CurrentScheduler.Timestamp;
#endif

    /// <summary>Gets the scheduler resolved for the current operation.</summary>
    private IScheduler CurrentScheduler => schedulerFactory();

#if REACTIVE_SHIM
    /// <inheritdoc/>
    public IDisposable Schedule<TState>(
        TState state,
        Func<IScheduler, TState, IDisposable> action) =>
        CurrentScheduler.Schedule(state, action);

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(
        TState state,
        TimeSpan dueTime,
        Func<IScheduler, TState, IDisposable> action) =>
        ScheduleWithDueTime(
            state,
            dueTime,
            action,
            static (
                scheduler,
                state,
                relativeDueTime,
                action) => scheduler.Schedule(state, relativeDueTime, action));

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(
        TState state,
        DateTimeOffset dueTime,
        Func<IScheduler, TState, IDisposable> action) =>
        ScheduleWithDueTime(
            state,
            dueTime,
            action,
            static (
                scheduler,
                state,
                absoluteDueTime,
                action) => scheduler.Schedule(state, absoluteDueTime, action));
#else
    /// <inheritdoc/>
    public void Schedule(IWorkItem item) => CurrentScheduler.Schedule(item);

    /// <inheritdoc/>
    public void Schedule(IWorkItem item, long dueTimestamp) => CurrentScheduler.Schedule(item, dueTimestamp);
#endif

#if REACTIVE_SHIM
    /// <summary>Schedules work through the selected delayed scheduling overload.</summary>
    /// <typeparam name="TState">The scheduled state type.</typeparam>
    /// <typeparam name="TDueTime">The scheduler due-time type.</typeparam>
    /// <param name="state">The scheduled state.</param>
    /// <param name="dueTime">The due time to pass to the selected scheduler overload.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="schedule">The selected scheduler overload.</param>
    /// <returns>A disposable that cancels the scheduled action.</returns>
    private IDisposable ScheduleWithDueTime<TState, TDueTime>(
        TState state,
        TDueTime dueTime,
        Func<IScheduler, TState, IDisposable> action,
        Func<IScheduler, TState, TDueTime, Func<IScheduler, TState, IDisposable>, IDisposable> schedule) =>
        schedule(CurrentScheduler, state, dueTime, action);
#endif
}
