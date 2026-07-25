// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno.Reactive.IoTDashboard.Models;

/// <summary>Identifies the type of signal produced by a simulated IoT device.</summary>
public enum SensorKind
{
    /// <summary>Temperature telemetry.</summary>
    Temperature,

    /// <summary>Pressure telemetry.</summary>
    Pressure,

    /// <summary>Vibration telemetry.</summary>
    Vibration,

    /// <summary>Power consumption telemetry.</summary>
    Energy,

    /// <summary>Security state telemetry.</summary>
    Security,

    /// <summary>Flow-rate telemetry.</summary>
    Flow
}
