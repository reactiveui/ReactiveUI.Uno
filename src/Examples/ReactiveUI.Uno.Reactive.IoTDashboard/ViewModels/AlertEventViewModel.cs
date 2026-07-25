// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Uno.Reactive.IoTDashboard.Models;

namespace ReactiveUI.Uno.Reactive.IoTDashboard.ViewModels;

/// <summary>Represents an alert shown in the operator alert feed.</summary>
public sealed class AlertEventViewModel : ReactiveObject
{
    /// <summary>Stores the local clock display format.</summary>
    private const string LocalClockFormat = "HH':'mm':'ss";

    /// <summary>Initializes a new instance of the <see cref="AlertEventViewModel"/> class.</summary>
    /// <param name="alert">The alert event.</param>
    public AlertEventViewModel(AlertEvent alert)
    {
        ArgumentNullException.ThrowIfNull(alert);
        Event = alert;
    }

    /// <summary>Gets the source alert event.</summary>
    public AlertEvent Event { get; }

    /// <summary>Gets the alert headline.</summary>
    public string Headline => $"{Event.DeviceName} - {Event.Status}";

    /// <summary>Gets the alert message.</summary>
    public string Message => Event.Message;

    /// <summary>Gets the formatted timestamp.</summary>
    public string TimestampText =>
        Event.Timestamp.ToLocalTime().ToString(LocalClockFormat, CultureInfo.InvariantCulture);

    /// <summary>Gets or sets a value indicating whether the alert was acknowledged.</summary>
    public bool IsAcknowledged
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
            RaiseDependentPropertyChanged(nameof(AcknowledgementText));
        }
    }

    /// <summary>Gets the acknowledgement state text.</summary>
    public string AcknowledgementText => IsAcknowledged ? "ACKNOWLEDGED" : "ACTIVE";

    /// <inheritdoc/>
    public override string ToString() => $"{TimestampText} {Headline}: {Message} ({AcknowledgementText})";

    /// <summary>Raises a dependent property change notification.</summary>
    /// <param name="propertyName">The property name to notify.</param>
    private void RaiseDependentPropertyChanged(string propertyName) =>
        ((IReactiveObject)this).RaisePropertyChanged(new(propertyName));
}
