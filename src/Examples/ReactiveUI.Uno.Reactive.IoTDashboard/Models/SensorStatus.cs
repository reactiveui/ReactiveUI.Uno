// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno.Reactive.IoTDashboard.Models;

/// <summary>Represents the live health state of a simulated device.</summary>
public enum SensorStatus
{
    /// <summary>The device is operating inside its normal range.</summary>
    Nominal = 0,

    /// <summary>The device is drifting toward an operator threshold.</summary>
    Attention = 1,

    /// <summary>The device is outside its safe operating range.</summary>
    Critical = 2,
}
