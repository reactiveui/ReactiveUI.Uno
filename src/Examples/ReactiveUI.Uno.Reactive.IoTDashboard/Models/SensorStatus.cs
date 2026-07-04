// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno.Reactive.IoTDashboard.Models;

/// <summary>Represents the live health state of a simulated device.</summary>
public enum SensorStatus
{
    /// <summary>The device is operating inside its normal range.</summary>
    Nominal,

    /// <summary>The device is drifting toward an operator threshold.</summary>
    Attention,

    /// <summary>The device is outside its safe operating range.</summary>
    Critical
}
