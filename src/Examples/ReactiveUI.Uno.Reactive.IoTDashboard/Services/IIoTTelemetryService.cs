// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Uno.Reactive.IoTDashboard.Models;

namespace ReactiveUI.Uno.Reactive.IoTDashboard.Services;

/// <summary>Provides live IoT telemetry for the dashboard sample.</summary>
/// <remarks>The boundary intentionally exposes only the BCL <see cref="IObservable{T}"/> interface.</remarks>
public interface IIoTTelemetryService
{
    /// <summary>Gets the shared stream of generated telemetry samples.</summary>
    IObservable<SensorReading> Readings { get; }

    /// <summary>Gets a point-in-time sample for every known device.</summary>
    /// <returns>The current readings for the simulated devices.</returns>
    IReadOnlyList<SensorReading> GetSnapshot();
}
