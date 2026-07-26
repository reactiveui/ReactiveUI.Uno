// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Uno.Reactive.IoTDashboard.Models;
using ReactiveUI.Uno.Reactive.IoTDashboard.Services;

namespace ReactiveUI.Uno.Reactive.IoTDashboard.ViewModels;

/// <summary>Coordinates the live IoT dashboard state and commands.</summary>
public sealed class DashboardViewModel : ReactiveObject, IRoutableViewModel, IDisposable
{
    /// <summary>Stores the text used when no active alert is available.</summary>
    private const string NoActiveAlertsText = "No active alerts.";

    /// <summary>Stores the local clock display format.</summary>
    private const string LocalClockFormat = "HH':'mm':'ss";

    /// <summary>Stores the simulated snapshot refresh delay.</summary>
    private const int SnapshotRefreshDelayMilliseconds = 150;

    /// <summary>Stores the maximum number of alerts retained in the feed.</summary>
    private const int MaximumAlertCount = 8;

    /// <summary>Stores the telemetry service.</summary>
    private readonly IIoTTelemetryService _telemetry;

    /// <summary>Stores the clock used for operator event timestamps.</summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>Publishes whether the alert command can execute.</summary>
    private readonly BehaviorSubject<bool> _canAcknowledgeAlert = new(false);

    /// <summary>Stores the live telemetry subscription.</summary>
    private readonly IDisposable _telemetrySubscription;

    /// <summary>Initializes a new instance of the <see cref="DashboardViewModel"/> class.</summary>
    /// <param name="hostScreen">The screen that hosts this route.</param>
    /// <param name="telemetry">The local telemetry service.</param>
    /// <param name="timeProvider">The clock used for operator event timestamps.</param>
    public DashboardViewModel(IScreen hostScreen, IIoTTelemetryService telemetry, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(hostScreen);
        ArgumentNullException.ThrowIfNull(telemetry);
        ArgumentNullException.ThrowIfNull(timeProvider);

        HostScreen = hostScreen;
        UrlPathSegment = "iot-dashboard";
        _telemetry = telemetry;
        _timeProvider = timeProvider;
        LastUpdated = _timeProvider.GetUtcNow();

        foreach (var reading in _telemetry.GetSnapshot())
        {
            Devices.Add(new(reading));
        }

        SelectedDevice = Devices.Count > 0 ? Devices[0] : null;
        if (SelectedDevice is not null)
        {
            SelectedDevice.IsSelected = true;
        }

        ToggleStreaming = ReactiveCommand.Create(ToggleStream);
        RefreshSnapshot = ReactiveCommand.CreateFromTask(RefreshSnapshotAsync);
        ResetSimulation = ReactiveCommand.Create(Reset);
        SelectDevice = ReactiveCommand.Create<DeviceTileViewModel>(Select);
        AcknowledgeAlert = ReactiveCommand.CreateFromObservable(
            AcknowledgeAlertObservable,
            _canAcknowledgeAlert);

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
        set
        {
            if (ReferenceEquals(field, value))
            {
                return;
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
            RaiseDependentPropertyChanged(nameof(SelectedDeviceSummary));
        }
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
        set
        {
            if (field == value)
            {
                return;
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
            RaiseDependentPropertyChanged(nameof(StreamStateText));
        }
    } = true;

    /// <summary>Gets the number of processed readings.</summary>
    public int TotalReadings
    {
        get;
        private set
        {
            if (field == value)
            {
                return;
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
            RaiseDependentPropertyChanged(nameof(SampleCountText));
        }
    }

    /// <summary>Gets the latest update timestamp.</summary>
    public DateTimeOffset LastUpdated
    {
        get;
        private set
        {
            if (field == value)
            {
                return;
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
            RaiseDependentPropertyChanged(nameof(LastUpdatedText));
        }
    }

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
        private set
        {
            if (field == value)
            {
                return;
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
            _canAcknowledgeAlert.OnNext(value);
        }
    }

    /// <summary>Gets the latest alert display text.</summary>
    public string LatestAlertText
    {
        get;
        private set => _ = this.RaiseAndSetIfChanged(ref field, value);
    } = NoActiveAlertsText;

    /// <summary>Gets the derived stream state text.</summary>
    public string StreamStateText => IsStreaming ? "Live stream running" : "Live stream paused";

    /// <summary>Gets the derived sample count text.</summary>
    public string SampleCountText =>
        string.Create(CultureInfo.InvariantCulture, $"{TotalReadings:N0} samples processed");

    /// <summary>Gets the derived selected device summary.</summary>
    public string SelectedDeviceSummary => SelectedDevice is null
        ? "Select a device"
        : $"{SelectedDevice.DisplayName} | {SelectedDevice.Kind} | {SelectedDevice.StatusText}";

    /// <summary>Gets the derived latest update text.</summary>
    public string LastUpdatedText =>
        $"Last update {LastUpdated.ToLocalTime().ToString(LocalClockFormat, CultureInfo.InvariantCulture)}";

    /// <summary>Gets the text describing how many devices match the current search text.</summary>
    public string FilterSummary
    {
        get
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                return $"{Devices.Count} devices visible";
            }

            var count = CountMatchingDevices(SearchText);
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
        ToggleStreaming.Dispose();
        RefreshSnapshot.Dispose();
        AcknowledgeAlert.Dispose();
        _canAcknowledgeAlert.Dispose();
        ResetSimulation.Dispose();
        SelectDevice.Dispose();
    }

    /// <summary>Counts devices whose display name matches the supplied search text.</summary>
    /// <param name="searchText">The search text.</param>
    /// <returns>The number of matching devices.</returns>
    private int CountMatchingDevices(string searchText)
    {
        var count = 0;
        foreach (var device in Devices)
        {
            if (device.DisplayName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                count++;
            }
        }

        return count;
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
        await Task.Delay(SnapshotRefreshDelayMilliseconds).ConfigureAwait(true);

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
        AlertEventViewModel? alert = null;
        foreach (var item in Alerts)
        {
            if (!item.IsAcknowledged)
            {
                alert = item;
                break;
            }
        }

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
        var acknowledgedAt = _timeProvider.GetUtcNow()
            .ToLocalTime()
            .ToString(LocalClockFormat, CultureInfo.InvariantCulture);
        InteractionMessage = $"Operator acknowledged {alert.Event.DeviceName} at {acknowledgedAt}.";
        UpdateAlertState($"Acknowledged {alert.Event.DeviceName}.");

        return Unit.Default;
    }

    /// <summary>Resets counters and alert state.</summary>
    private void Reset()
    {
        Alerts.Clear();
        TotalReadings = 0;
        HasActiveAlert = false;
        LatestAlertText = NoActiveAlertsText;
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
        DeviceTileViewModel? device = null;
        foreach (var tile in Devices)
        {
            if (tile.DeviceId == reading.DeviceId)
            {
                device = tile;
                break;
            }
        }

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
            LatestAlertText = NoActiveAlertsText;
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
        var message = string.Create(
            CultureInfo.InvariantCulture,
            $"{reading.DeviceName} reported {reading.Value:0.0} {reading.Unit}.");
        var alert = new AlertEventViewModel(new(reading.DeviceName, message, reading.Status, reading.Timestamp));

        Alerts.Insert(0, alert);
        while (Alerts.Count > MaximumAlertCount)
        {
            Alerts.RemoveAt(Alerts.Count - 1);
        }

        UpdateAlertState(message);
    }

    /// <summary>Updates the aggregate alert state.</summary>
    /// <param name="latestAlertText">The latest alert text.</param>
    private void UpdateAlertState(string latestAlertText)
    {
        HasActiveAlert = false;
        foreach (var item in Alerts)
        {
            if (!item.IsAcknowledged)
            {
                HasActiveAlert = true;
                break;
            }
        }

        LatestAlertText = HasActiveAlert ? latestAlertText : NoActiveAlertsText;
    }

    /// <summary>Raises a dependent property change notification.</summary>
    /// <param name="propertyName">The property name to notify.</param>
    private void RaiseDependentPropertyChanged(string propertyName) =>
        ((IReactiveObject)this).RaisePropertyChanged(new(propertyName));
}
