// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
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
    public DateTimeOffset Now => schedulerFactory().Now;

#if REACTIVE_SHIM
    /// <inheritdoc/>
    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action) => schedulerFactory().Schedule(state, action);

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action) => schedulerFactory().Schedule(state, dueTime, action);

    /// <inheritdoc/>
    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action) => schedulerFactory().Schedule(state, dueTime, action);
#else
    /// <inheritdoc/>
    public long Timestamp => schedulerFactory().Timestamp;

    /// <inheritdoc/>
    public void Schedule(IWorkItem item) => schedulerFactory().Schedule(item);

    /// <inheritdoc/>
    public void Schedule(IWorkItem item, long dueTimestamp) => schedulerFactory().Schedule(item, dueTimestamp);
#endif
}
