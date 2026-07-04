// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Uno.Reactive.IoTDashboard.Models;

namespace ReactiveUI.Uno.Reactive.IoTDashboard.Services;

/// <summary>Generates deterministic local telemetry for the IoT dashboard sample.</summary>
public sealed class LocalIoTTelemetryService : IIoTTelemetryService
{
    /// <summary>Stores the simulated devices.</summary>
    private readonly DeviceProfile[] _devices =
    [
        new("line-oven", "Line 4 curing oven", SensorKind.Temperature, 72, 8, 1.8, "deg C", 82, 88),
        new("pump-a17", "Pump A17 pressure", SensorKind.Pressure, 42, 7, 1.2, "bar", 51, 56),
        new("mill-vibe", "Mixer vibration", SensorKind.Vibration, 3.2, 1.3, 0.35, "mm/s", 4.5, 5.2),
        new("solar-01", "Solar inverter", SensorKind.Energy, 68, 18, 2.5, "kW", 91, 96),
        new("gate-03", "Loading bay door", SensorKind.Security, 0, 1, 0, "state", 0.75, 0.95),
        new("coolant", "Coolant loop flow", SensorKind.Flow, 118, 16, 2.2, "L/min", 92, 84, LowerIsWorse: true)
    ];

    /// <summary>Initializes a new instance of the <see cref="LocalIoTTelemetryService"/> class.</summary>
    public LocalIoTTelemetryService() =>
        Readings = Observable.Interval(TimeSpan.FromMilliseconds(650), ReactiveUI.Reactive.RxSchedulers.TaskpoolScheduler)
            .Select(CreateReading)
            .Publish()
            .RefCount();

    /// <inheritdoc/>
    public IObservable<SensorReading> Readings { get; }

    /// <inheritdoc/>
    public IReadOnlyList<SensorReading> GetSnapshot() =>
        _devices.Select((device, index) => CreateReading(index)).ToArray();

    /// <summary>Creates a telemetry sample for a simulated device.</summary>
    /// <param name="tick">The stream tick used to choose a device and generate a wave.</param>
    /// <returns>The generated telemetry reading.</returns>
    private SensorReading CreateReading(long tick)
    {
        var index = (int)(Math.Abs(tick) % _devices.Length);
        var device = _devices[index];
        var wave = Math.Sin((tick + (index * 7)) * 0.37);
        var jitter = Math.Sin((tick + (index * 11)) * 1.91) * device.Noise * 0.5;
        var securityValue = Convert.ToDouble(((tick + index) % 19) == 0);
        var signalValue = Math.Round(device.Baseline + (wave * device.Amplitude) + jitter, 1);
        var value = device.Kind == SensorKind.Security ? securityValue : signalValue;

        return new(
            device.DeviceId,
            device.DeviceName,
            device.Kind,
            value,
            device.Unit,
            device.GetStatus(value),
            DateTimeOffset.Now);
    }

    /// <summary>Describes one simulated device profile.</summary>
    /// <param name="DeviceId">The stable device identifier.</param>
    /// <param name="DeviceName">The display device name.</param>
    /// <param name="Kind">The sensor kind.</param>
    /// <param name="Baseline">The nominal value.</param>
    /// <param name="Amplitude">The signal wave amplitude.</param>
    /// <param name="Noise">The deterministic jitter range.</param>
    /// <param name="Unit">The display unit.</param>
    /// <param name="AttentionThreshold">The warning threshold.</param>
    /// <param name="CriticalThreshold">The critical threshold.</param>
    /// <param name="LowerIsWorse">A value indicating whether lower values are worse for this signal.</param>
    private sealed record DeviceProfile(
        string DeviceId,
        string DeviceName,
        SensorKind Kind,
        double Baseline,
        double Amplitude,
        double Noise,
        string Unit,
        double AttentionThreshold,
        double CriticalThreshold,
        bool LowerIsWorse = false)
    {
        /// <summary>Computes the threshold status for the supplied value.</summary>
        /// <param name="value">The current value.</param>
        /// <returns>The computed sensor status.</returns>
        public SensorStatus GetStatus(double value)
        {
            if (LowerIsWorse)
            {
                if (value <= CriticalThreshold)
                {
                    return SensorStatus.Critical;
                }

                return value <= AttentionThreshold ? SensorStatus.Attention : SensorStatus.Nominal;
            }

            if (value >= CriticalThreshold)
            {
                return SensorStatus.Critical;
            }

            return value >= AttentionThreshold ? SensorStatus.Attention : SensorStatus.Nominal;
        }
    }
}
