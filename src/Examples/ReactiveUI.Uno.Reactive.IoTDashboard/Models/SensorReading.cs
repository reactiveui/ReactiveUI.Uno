// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno.Reactive.IoTDashboard.Models;

/// <summary>Represents a single simulated IoT telemetry sample.</summary>
/// <param name="DeviceId">The stable device identifier.</param>
/// <param name="DeviceName">The operator-friendly device name.</param>
/// <param name="Kind">The sensor signal type.</param>
/// <param name="Value">The measured value.</param>
/// <param name="Unit">The display unit for <paramref name="Value"/>.</param>
/// <param name="Status">The threshold state computed for the value.</param>
/// <param name="Timestamp">The timestamp when the sample was generated.</param>
public sealed record SensorReading(
    string DeviceId,
    string DeviceName,
    SensorKind Kind,
    double Value,
    string Unit,
    SensorStatus Status,
    DateTimeOffset Timestamp);
