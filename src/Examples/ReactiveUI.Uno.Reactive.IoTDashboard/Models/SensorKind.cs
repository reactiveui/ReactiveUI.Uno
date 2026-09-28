// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno.Reactive.IoTDashboard.Models;

/// <summary>Identifies the type of signal produced by a simulated IoT device.</summary>
public enum SensorKind
{
    /// <summary>Temperature telemetry.</summary>
    Temperature = 0,

    /// <summary>Pressure telemetry.</summary>
    Pressure = 1,

    /// <summary>Vibration telemetry.</summary>
    Vibration = 2,

    /// <summary>Power consumption telemetry.</summary>
    Energy = 3,

    /// <summary>Security state telemetry.</summary>
    Security = 4,

    /// <summary>Flow-rate telemetry.</summary>
    Flow = 5,
}
