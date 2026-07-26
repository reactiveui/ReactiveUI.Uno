// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno.Reactive.IoTDashboard.Models;

/// <summary>Represents an operator-facing alert raised from a telemetry sample.</summary>
/// <param name="DeviceName">The device that raised the alert.</param>
/// <param name="Message">The operator-facing alert message.</param>
/// <param name="Status">The severity of the alert.</param>
/// <param name="Timestamp">The timestamp when the alert was raised.</param>
public sealed record AlertEvent(string DeviceName, string Message, SensorStatus Status, DateTimeOffset Timestamp);
