// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using ReactiveUI.Primitives.Concurrency;

namespace ReactiveUI.Uno;

internal sealed class SchedulerSequencerAdapter(IScheduler scheduler) : ISequencer
{
    public DateTimeOffset Now => scheduler.Now;

    public long Timestamp => Stopwatch.GetTimestamp();

    public void Schedule(IWorkItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        scheduler.Schedule(item, static (_, workItem) =>
        {
            workItem.Execute();
            return Disposable.Empty;
        });
    }

    public void Schedule(IWorkItem item, long dueTimestamp)
    {
        ArgumentNullException.ThrowIfNull(item);

        var timestamp = Timestamp;
        var dueTime = dueTimestamp <= timestamp ? TimeSpan.Zero : Stopwatch.GetElapsedTime(timestamp, dueTimestamp);
        scheduler.Schedule(item, dueTime, static (_, workItem) =>
        {
            workItem.Execute();
            return Disposable.Empty;
        });
    }
}
