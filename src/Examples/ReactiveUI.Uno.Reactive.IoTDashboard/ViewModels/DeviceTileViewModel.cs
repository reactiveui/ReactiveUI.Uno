// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Uno.Reactive.IoTDashboard.Models;

namespace ReactiveUI.Uno.Reactive.IoTDashboard.ViewModels;

/// <summary>Represents one device tile on the dashboard.</summary>
public sealed class DeviceTileViewModel : ReactiveObject
{
    /// <summary>Stores the local clock display format.</summary>
    private const string LocalClockFormat = "HH':'mm':'ss";

    /// <summary>Initializes a new instance of the <see cref="DeviceTileViewModel"/> class.</summary>
    /// <param name="reading">The initial device reading.</param>
    public DeviceTileViewModel(SensorReading reading)
    {
        ArgumentNullException.ThrowIfNull(reading);

        DeviceId = reading.DeviceId;
        DisplayName = reading.DeviceName;
        Kind = reading.Kind;
        Unit = reading.Unit;
        Value = reading.Value;
        Status = reading.Status;
        Timestamp = reading.Timestamp;
    }

    /// <summary>Gets the stable device identifier.</summary>
    public string DeviceId { get; }

    /// <summary>Gets the display name.</summary>
    public string DisplayName { get; }

    /// <summary>Gets the sensor kind.</summary>
    public SensorKind Kind { get; }

    /// <summary>Gets the display unit.</summary>
    public string Unit { get; }

    /// <summary>Gets the latest telemetry value.</summary>
    public double Value
    {
        get;
        private set
        {
            if (field.Equals(value))
            {
                return;
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
            RaiseDependentPropertyChanged(nameof(ValueText));
        }
    }

    /// <summary>Gets the latest threshold status.</summary>
    public SensorStatus Status
    {
        get;
        private set
        {
            if (field == value)
            {
                return;
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
            RaiseDependentPropertyChanged(nameof(StatusText));
        }
    }

    /// <summary>Gets the latest telemetry timestamp.</summary>
    public DateTimeOffset Timestamp
    {
        get;
        private set
        {
            if (field == value)
            {
                return;
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
            RaiseDependentPropertyChanged(nameof(TimestampText));
        }
    }

    /// <summary>Gets the delta from the previous value.</summary>
    public double Trend
    {
        get;
        private set
        {
            if (field.Equals(value))
            {
                return;
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
            RaiseDependentPropertyChanged(nameof(TrendText));
        }
    }

    /// <summary>Gets or sets a value indicating whether this tile is selected.</summary>
    public bool IsSelected
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
            RaiseDependentPropertyChanged(nameof(SelectionText));
        }
    }

    /// <summary>Gets the formatted value.</summary>
    public string ValueText => FormatValue(Value, Unit);

    /// <summary>Gets the formatted trend text.</summary>
    public string TrendText => string.Create(CultureInfo.InvariantCulture, $"{Trend:+0.0;-0.0;0.0}");

    /// <summary>Gets the formatted timestamp.</summary>
    public string TimestampText => Timestamp.ToLocalTime().ToString(LocalClockFormat, CultureInfo.InvariantCulture);

    /// <summary>Gets the display status text.</summary>
    public string StatusText => Status.ToString();

    /// <summary>Gets the selected marker text.</summary>
    public string SelectionText => IsSelected ? "SELECTED" : "MONITORING";

    /// <summary>Applies a new telemetry reading to the tile.</summary>
    /// <param name="reading">The reading to apply.</param>
    public void Apply(SensorReading reading)
    {
        ArgumentNullException.ThrowIfNull(reading);

        Trend = reading.Value - Value;
        Value = reading.Value;
        Status = reading.Status;
        Timestamp = reading.Timestamp;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{DisplayName}: {ValueText}";

    /// <summary>Formats a sensor value with its display unit.</summary>
    /// <param name="value">The sensor value.</param>
    /// <param name="unit">The sensor unit.</param>
    /// <returns>The formatted value.</returns>
    private static string FormatValue(double value, string unit) =>
        string.Create(CultureInfo.InvariantCulture, $"{value:0.0} {unit}");

    /// <summary>Raises a dependent property change notification.</summary>
    /// <param name="propertyName">The property name to notify.</param>
    private void RaiseDependentPropertyChanged(string propertyName) =>
        ((IReactiveObject)this).RaisePropertyChanged(new(propertyName));
}
