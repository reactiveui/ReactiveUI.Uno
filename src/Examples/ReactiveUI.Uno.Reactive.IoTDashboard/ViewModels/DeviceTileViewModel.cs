// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Uno.Reactive.IoTDashboard.Models;

namespace ReactiveUI.Uno.Reactive.IoTDashboard.ViewModels;

/// <summary>Represents one device tile on the dashboard.</summary>
public sealed class DeviceTileViewModel : ReactiveObject, IDisposable
{
    /// <summary>Stores the formatted telemetry value.</summary>
    private readonly ObservableAsPropertyHelper<string> _valueText;

    /// <summary>Stores the formatted telemetry trend.</summary>
    private readonly ObservableAsPropertyHelper<string> _trendText;

    /// <summary>Stores the formatted timestamp.</summary>
    private readonly ObservableAsPropertyHelper<string> _timestampText;

    /// <summary>Stores the display status text.</summary>
    private readonly ObservableAsPropertyHelper<string> _statusText;

    /// <summary>Stores the selected marker text.</summary>
    private readonly ObservableAsPropertyHelper<string> _selectionText;

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

        _valueText = this.WhenAnyValue(x => x.Value, x => x.Unit, FormatValue)
            .ToProperty(this, static x => x.ValueText);

        _trendText = this.WhenAnyValue(x => x.Trend)
            .Select(static trend => string.Create(CultureInfo.InvariantCulture, $"{trend:+0.0;-0.0;0.0}"))
            .ToProperty(this, static x => x.TrendText);

        _timestampText = this.WhenAnyValue(x => x.Timestamp)
            .Select(static timestamp => timestamp.ToLocalTime().ToString("HH:mm:ss", CultureInfo.CurrentCulture))
            .ToProperty(this, static x => x.TimestampText);

        _statusText = this.WhenAnyValue(x => x.Status)
            .Select(static status => status.ToString())
            .ToProperty(this, static x => x.StatusText);

        _selectionText = this.WhenAnyValue(x => x.IsSelected)
            .Select(static isSelected => isSelected ? "SELECTED" : "MONITORING")
            .ToProperty(this, static x => x.SelectionText);
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
        private set => _ = this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the latest threshold status.</summary>
    public SensorStatus Status
    {
        get;
        private set => _ = this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the latest telemetry timestamp.</summary>
    public DateTimeOffset Timestamp
    {
        get;
        private set => _ = this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the delta from the previous value.</summary>
    public double Trend
    {
        get;
        private set => _ = this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets a value indicating whether this tile is selected.</summary>
    public bool IsSelected
    {
        get;
        set => _ = this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the formatted value.</summary>
    public string ValueText => _valueText.Value;

    /// <summary>Gets the formatted trend text.</summary>
    public string TrendText => _trendText.Value;

    /// <summary>Gets the formatted timestamp.</summary>
    public string TimestampText => _timestampText.Value;

    /// <summary>Gets the display status text.</summary>
    public string StatusText => _statusText.Value;

    /// <summary>Gets the selected marker text.</summary>
    public string SelectionText => _selectionText.Value;

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
    public void Dispose()
    {
        _valueText.Dispose();
        _trendText.Dispose();
        _timestampText.Dispose();
        _statusText.Dispose();
        _selectionText.Dispose();
    }

    /// <inheritdoc/>
    public override string ToString() => $"{DisplayName}: {ValueText}";

    /// <summary>Formats a sensor value with its display unit.</summary>
    /// <param name="value">The sensor value.</param>
    /// <param name="unit">The sensor unit.</param>
    /// <returns>The formatted value.</returns>
    private static string FormatValue(double value, string unit) =>
        string.Create(CultureInfo.InvariantCulture, $"{value:0.0} {unit}");
}
