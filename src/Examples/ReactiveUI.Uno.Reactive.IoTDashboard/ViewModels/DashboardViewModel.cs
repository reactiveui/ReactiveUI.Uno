// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using ReactiveUI.Uno.Reactive.IoTDashboard.Models;
using ReactiveUI.Uno.Reactive.IoTDashboard.Services;

namespace ReactiveUI.Uno.Reactive.IoTDashboard.ViewModels;

/// <summary>Coordinates the live IoT dashboard state and commands.</summary>
public sealed class DashboardViewModel : ReactiveObject, IRoutableViewModel, IDisposable
{
    /// <summary>Stores the telemetry service.</summary>
    private readonly IIoTTelemetryService _telemetry;

    /// <summary>Stores the live telemetry subscription.</summary>
    private readonly IDisposable _telemetrySubscription;

    /// <summary>Stores the derived stream state text.</summary>
    private readonly ObservableAsPropertyHelper<string> _streamStateText;

    /// <summary>Stores the derived sample count text.</summary>
    private readonly ObservableAsPropertyHelper<string> _sampleCountText;

    /// <summary>Stores the derived selected device text.</summary>
    private readonly ObservableAsPropertyHelper<string> _selectedDeviceSummary;

    /// <summary>Stores the derived latest update text.</summary>
    private readonly ObservableAsPropertyHelper<string> _lastUpdatedText;

    /// <summary>Initializes a new instance of the <see cref="DashboardViewModel"/> class.</summary>
    /// <param name="hostScreen">The screen that hosts this route.</param>
    /// <param name="telemetry">The local telemetry service.</param>
    public DashboardViewModel(IScreen hostScreen, IIoTTelemetryService telemetry)
    {
        ArgumentNullException.ThrowIfNull(hostScreen);
        ArgumentNullException.ThrowIfNull(telemetry);

        HostScreen = hostScreen;
        UrlPathSegment = "iot-dashboard";
        _telemetry = telemetry;

        foreach (var reading in _telemetry.GetSnapshot())
        {
            Devices.Add(new(reading));
        }

        SelectedDevice = Devices.FirstOrDefault();
        if (SelectedDevice is not null)
        {
            SelectedDevice.IsSelected = true;
        }

        _streamStateText = this.WhenAnyValue(x => x.IsStreaming)
            .Select(static isStreaming => isStreaming ? "Live stream running" : "Live stream paused")
            .ToProperty(this, static x => x.StreamStateText);

        _sampleCountText = this.WhenAnyValue(x => x.TotalReadings)
            .Select(static count => string.Create(CultureInfo.InvariantCulture, $"{count:N0} samples processed"))
            .ToProperty(this, static x => x.SampleCountText);

        _selectedDeviceSummary = this.WhenAnyValue(x => x.SelectedDevice)
            .Select(static device => device is null ? "Select a device" : $"{device.DisplayName} | {device.Kind} | {device.StatusText}")
            .ToProperty(this, static x => x.SelectedDeviceSummary);

        _lastUpdatedText = this.WhenAnyValue(x => x.LastUpdated)
            .Select(static timestamp => $"Last update {timestamp.ToLocalTime():HH:mm:ss}")
            .ToProperty(this, static x => x.LastUpdatedText);

        ToggleStreaming = ReactiveCommand.Create(ToggleStream);
        RefreshSnapshot = ReactiveCommand.CreateFromTask(RefreshSnapshotAsync);
        ResetSimulation = ReactiveCommand.Create(Reset);
        SelectDevice = ReactiveCommand.Create<DeviceTileViewModel>(Select);
        AcknowledgeAlert = ReactiveCommand.CreateFromObservable(AcknowledgeAlertObservable, this.WhenAnyValue(x => x.HasActiveAlert));

        _telemetrySubscription = _telemetry.Readings
            .Where(_ => IsStreaming)
            .ObserveOn(ReactiveUI.Reactive.RxSchedulers.MainThreadScheduler)
            .Subscribe(ApplyReading, exception => StatusMessage = exception.Message);
    }

    /// <inheritdoc/>
    public string UrlPathSegment { get; }

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the device tiles displayed by the dashboard.</summary>
    public ObservableCollection<DeviceTileViewModel> Devices { get; } = [];

    /// <summary>Gets the alert feed displayed by the dashboard.</summary>
    public ObservableCollection<AlertEventViewModel> Alerts { get; } = [];

    /// <summary>Gets the interaction raised when an operator acknowledges an alert.</summary>
    public Interaction<AlertEvent, bool> ConfirmAcknowledge { get; } = new();

    /// <summary>Gets or sets the selected device.</summary>
    public DeviceTileViewModel? SelectedDevice
    {
        get;
        set => _ = this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the search text used to demonstrate two-way binding.</summary>
    public string SearchText
    {
        get;
        set
        {
            if (string.Equals(field, value, StringComparison.Ordinal))
            {
                return;
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
            RaiseDependentPropertyChanged(nameof(FilterSummary));
        }
    } = string.Empty;

    /// <summary>Gets or sets a value indicating whether live telemetry is applied to the view model.</summary>
    public bool IsStreaming
    {
        get;
        set => _ = this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    /// <summary>Gets the number of processed readings.</summary>
    public int TotalReadings
    {
        get;
        private set => _ = this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the latest update timestamp.</summary>
    public DateTimeOffset LastUpdated
    {
        get;
        private set => _ = this.RaiseAndSetIfChanged(ref field, value);
    } = DateTimeOffset.Now;

    /// <summary>Gets the current operator status message.</summary>
    public string StatusMessage
    {
        get;
        private set => _ = this.RaiseAndSetIfChanged(ref field, value);
    } = "Live stream initialized.";

    /// <summary>Gets or sets the latest interaction result message.</summary>
    public string InteractionMessage
    {
        get;
        set => _ = this.RaiseAndSetIfChanged(ref field, value);
    } = "No operator interaction yet.";

    /// <summary>Gets a value indicating whether an unacknowledged alert exists.</summary>
    public bool HasActiveAlert
    {
        get;
        private set => _ = this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the latest alert display text.</summary>
    public string LatestAlertText
    {
        get;
        private set => _ = this.RaiseAndSetIfChanged(ref field, value);
    } = "No active alerts.";

    /// <summary>Gets the derived stream state text.</summary>
    public string StreamStateText => _streamStateText.Value;

    /// <summary>Gets the derived sample count text.</summary>
    public string SampleCountText => _sampleCountText.Value;

    /// <summary>Gets the derived selected device summary.</summary>
    public string SelectedDeviceSummary => _selectedDeviceSummary.Value;

    /// <summary>Gets the derived latest update text.</summary>
    public string LastUpdatedText => _lastUpdatedText.Value;

    /// <summary>Gets the text describing how many devices match the current search text.</summary>
    public string FilterSummary
    {
        get
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                return $"{Devices.Count} devices visible";
            }

            var count = Devices.Count(device => device.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            return $"{count} devices match '{SearchText}'";
        }
    }

    /// <summary>Gets the command that pauses or resumes live telemetry.</summary>
    public ReactiveCommand<Unit, Unit> ToggleStreaming { get; }

    /// <summary>Gets the command that refreshes every tile from a point-in-time snapshot.</summary>
    public ReactiveCommand<Unit, Unit> RefreshSnapshot { get; }

    /// <summary>Gets the command that acknowledges the latest active alert.</summary>
    public ReactiveCommand<Unit, Unit> AcknowledgeAlert { get; }

    /// <summary>Gets the command that resets counters and alert state.</summary>
    public ReactiveCommand<Unit, Unit> ResetSimulation { get; }

    /// <summary>Gets the command that selects a device tile.</summary>
    public ReactiveCommand<DeviceTileViewModel, Unit> SelectDevice { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        _telemetrySubscription.Dispose();
        _streamStateText.Dispose();
        _sampleCountText.Dispose();
        _selectedDeviceSummary.Dispose();
        _lastUpdatedText.Dispose();
        ToggleStreaming.Dispose();
        RefreshSnapshot.Dispose();
        AcknowledgeAlert.Dispose();
        ResetSimulation.Dispose();
        SelectDevice.Dispose();

        foreach (var device in Devices)
        {
            device.Dispose();
        }
    }

    /// <summary>Toggles the live stream state.</summary>
    private void ToggleStream()
    {
        IsStreaming = !IsStreaming;
        StatusMessage = IsStreaming ? "Live telemetry resumed." : "Telemetry updates paused.";
    }

    /// <summary>Refreshes every tile from a point-in-time service snapshot.</summary>
    /// <returns>A task that represents the refresh operation.</returns>
    private async Task RefreshSnapshotAsync()
    {
        StatusMessage = "Refreshing device snapshot...";
        await Task.Delay(150).ConfigureAwait(true);

        foreach (var reading in _telemetry.GetSnapshot())
        {
            ApplyReading(reading);
        }

        StatusMessage = "Snapshot refreshed from local generator.";
    }

    /// <summary>Acknowledges the most recent active alert.</summary>
    /// <returns>An observable that completes when the acknowledgement flow is complete.</returns>
    private IObservable<Unit> AcknowledgeAlertObservable()
    {
        var alert = Alerts.FirstOrDefault(static item => !item.IsAcknowledged);
        return alert is null
            ? Observable.Return(Unit.Default)
            : ConfirmAcknowledge.Handle(alert.Event)
                .Select(approved => CompleteAcknowledge(alert, approved));
    }

    /// <summary>Completes an alert acknowledgement after the view handles the interaction.</summary>
    /// <param name="alert">The alert being acknowledged.</param>
    /// <param name="approved">A value indicating whether acknowledgement was approved.</param>
    /// <returns>The command completion value.</returns>
    private Unit CompleteAcknowledge(AlertEventViewModel alert, bool approved)
    {
        if (!approved)
        {
            return Unit.Default;
        }

        alert.IsAcknowledged = true;
        InteractionMessage = $"Operator acknowledged {alert.Event.DeviceName} at {DateTimeOffset.Now.ToLocalTime():HH:mm:ss}.";
        UpdateAlertState($"Acknowledged {alert.Event.DeviceName}.");

        return Unit.Default;
    }

    /// <summary>Resets counters and alert state.</summary>
    private void Reset()
    {
        Alerts.Clear();
        TotalReadings = 0;
        HasActiveAlert = false;
        LatestAlertText = "No active alerts.";
        InteractionMessage = "Simulation reset.";
        StatusMessage = "Dashboard state reset.";
    }

    /// <summary>Selects the supplied device.</summary>
    /// <param name="device">The device to select.</param>
    private void Select(DeviceTileViewModel device)
    {
        ArgumentNullException.ThrowIfNull(device);

        foreach (var tile in Devices)
        {
            tile.IsSelected = ReferenceEquals(tile, device);
        }

        SelectedDevice = device;
        StatusMessage = $"Selected {device.DisplayName}.";
    }

    /// <summary>Applies one live telemetry reading to the dashboard.</summary>
    /// <param name="reading">The telemetry reading.</param>
    private void ApplyReading(SensorReading reading)
    {
        var device = Devices.FirstOrDefault(tile => tile.DeviceId == reading.DeviceId);
        if (device is null)
        {
            return;
        }

        device.Apply(reading);
        TotalReadings++;
        LastUpdated = reading.Timestamp;

        if (reading.Status == SensorStatus.Critical)
        {
            AddAlert(reading);
        }
        else if (!HasActiveAlert)
        {
            LatestAlertText = "No active alerts.";
        }

        StatusMessage = $"{reading.DeviceName} published {device.ValueText}.";
        if (!ReferenceEquals(SelectedDevice, device))
        {
            return;
        }

        RaiseDependentPropertyChanged(nameof(SelectedDeviceSummary));
    }

    /// <summary>Adds an alert generated from a critical reading.</summary>
    /// <param name="reading">The critical reading.</param>
    private void AddAlert(SensorReading reading)
    {
        var message = string.Create(CultureInfo.InvariantCulture, $"{reading.DeviceName} reported {reading.Value:0.0} {reading.Unit}.");
        var alert = new AlertEventViewModel(new(reading.DeviceName, message, reading.Status, reading.Timestamp));

        Alerts.Insert(0, alert);
        while (Alerts.Count > 8)
        {
            Alerts.RemoveAt(Alerts.Count - 1);
        }

        UpdateAlertState(message);
    }

    /// <summary>Updates the aggregate alert state.</summary>
    /// <param name="latestAlertText">The latest alert text.</param>
    private void UpdateAlertState(string latestAlertText)
    {
        HasActiveAlert = Alerts.Any(static item => !item.IsAcknowledged);
        LatestAlertText = HasActiveAlert ? latestAlertText : "No active alerts.";
    }

    /// <summary>Raises a dependent property change notification.</summary>
    /// <param name="propertyName">The property name to notify.</param>
    private void RaiseDependentPropertyChanged(string propertyName) =>
        ((IReactiveObject)this).RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
}
