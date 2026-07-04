// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Text;
using ReactiveUI.Uno.Reactive.IoTDashboard.Models;
using ReactiveUI.Uno.Reactive.IoTDashboard.ViewModels;
using Windows.UI;

namespace ReactiveUI.Uno.Reactive.IoTDashboard.Views;

/// <summary>Displays the live IoT dashboard sample.</summary>
public sealed partial class DashboardView : DashboardViewBase
{
    /// <summary>Tracks whether device cards have been created.</summary>
    private bool _deviceCardsCreated;

    /// <summary>Initializes a new instance of the <see cref="DashboardView"/> class.</summary>
    public DashboardView()
    {
        BuildLayout();

        _ = this.WhenActivated(disposables =>
        {
            if (ViewModel is not null)
            {
                EnsureDeviceCards(ViewModel);
                AlertList.ItemsSource = ViewModel.Alerts;
            }

            disposables(this.Bind(ViewModel, static vm => vm.SearchText, static view => view.SearchBox.Text));
            disposables(this.BindCommand(ViewModel, static vm => vm.ToggleStreaming, static view => view.ToggleStreamButton));
            disposables(this.BindCommand(ViewModel, static vm => vm.RefreshSnapshot, static view => view.RefreshButton));
            disposables(this.BindCommand(ViewModel, static vm => vm.AcknowledgeAlert, static view => view.AcknowledgeButton));
            disposables(this.BindCommand(ViewModel, static vm => vm.ResetSimulation, static view => view.ResetButton));
            disposables(this.OneWayBind(ViewModel, static vm => vm.StreamStateText, static view => view.StreamStateText.Text));
            disposables(this.OneWayBind(ViewModel, static vm => vm.SampleCountText, static view => view.SampleCountText.Text));
            disposables(this.OneWayBind(ViewModel, static vm => vm.SelectedDeviceSummary, static view => view.SelectedDeviceText.Text));
            disposables(this.OneWayBind(ViewModel, static vm => vm.LastUpdatedText, static view => view.LastUpdatedText.Text));
            disposables(this.OneWayBind(ViewModel, static vm => vm.FilterSummary, static view => view.FilterSummaryText.Text));
            disposables(this.OneWayBind(ViewModel, static vm => vm.LatestAlertText, static view => view.LatestAlertText.Text));
            disposables(this.OneWayBind(ViewModel, static vm => vm.InteractionMessage, static view => view.InteractionMessageText.Text));
            disposables(this.OneWayBind(ViewModel, static vm => vm.StatusMessage, static view => view.StatusMessageText.Text));
            disposables(this.BindInteraction(ViewModel, static vm => vm.ConfirmAcknowledge, HandleAcknowledgeAsync));
        });
    }

    /// <summary>Gets the grid that hosts device cards.</summary>
    private Grid DeviceGrid { get; } = new()
    {
        ColumnSpacing = 8,
        RowSpacing = 8
    };

    /// <summary>Gets the alert list.</summary>
    private ListView AlertList { get; } = new()
    {
        MaxHeight = 160
    };

    /// <summary>Gets the search text box.</summary>
    private TextBox SearchBox { get; } = new()
    {
        PlaceholderText = "Filter devices",
        Margin = new(0, 4, 0, 0)
    };

    /// <summary>Gets the stream state text.</summary>
    private TextBlock StreamStateText { get; } = CreateText("Live stream running", 16, FontWeights.SemiBold);

    /// <summary>Gets the sample count text.</summary>
    private TextBlock SampleCountText { get; } = CreateText("0 samples processed", 16, FontWeights.SemiBold);

    /// <summary>Gets the selected device text.</summary>
    private TextBlock SelectedDeviceText { get; } = CreateText("Select a device", 15, FontWeights.Normal);

    /// <summary>Gets the latest update text.</summary>
    private TextBlock LastUpdatedText { get; } = CreateText("Last update --:--:--", 15, FontWeights.Normal);

    /// <summary>Gets the filter summary text.</summary>
    private TextBlock FilterSummaryText { get; } = CreateText("6 devices visible", 13, FontWeights.Normal);

    /// <summary>Gets the latest alert text.</summary>
    private TextBlock LatestAlertText { get; } = CreateText("No active alerts.", 15, FontWeights.SemiBold);

    /// <summary>Gets the interaction message text.</summary>
    private TextBlock InteractionMessageText { get; } = CreateText("No operator interaction yet.", 13, FontWeights.Normal);

    /// <summary>Gets the status message text.</summary>
    private TextBlock StatusMessageText { get; } = CreateText("Ready", 13, FontWeights.Normal);

    /// <summary>Gets the stream toggle button.</summary>
    private Button ToggleStreamButton { get; } = new() { Content = "Pause / Resume" };

    /// <summary>Gets the snapshot refresh button.</summary>
    private Button RefreshButton { get; } = new() { Content = "Refresh Snapshot" };

    /// <summary>Gets the acknowledge alert button.</summary>
    private Button AcknowledgeButton { get; } = new() { Content = "Acknowledge Alert" };

    /// <summary>Gets the reset button.</summary>
    private Button ResetButton { get; } = new() { Content = "Reset" };

    /// <summary>Handles an alert acknowledgement interaction.</summary>
    /// <param name="context">The interaction context.</param>
    /// <returns>A task that represents the interaction handling operation.</returns>
    private static Task HandleAcknowledgeAsync(IInteractionContext<AlertEvent, bool> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.SetOutput(true);
        return Task.CompletedTask;
    }

    /// <summary>Creates a text block with dashboard styling.</summary>
    /// <param name="text">The initial text.</param>
    /// <param name="fontSize">The font size.</param>
    /// <param name="fontWeight">The font weight.</param>
    /// <returns>The configured text block.</returns>
    private static TextBlock CreateText(string text, double fontSize, Windows.UI.Text.FontWeight fontWeight) =>
        new()
        {
            Text = text,
            FontSize = fontSize,
            FontWeight = fontWeight,
            Foreground = CreateBrush(Microsoft.UI.Colors.White),
            TextWrapping = TextWrapping.Wrap
        };

    /// <summary>Creates a solid color brush.</summary>
    /// <param name="color">The color to apply.</param>
    /// <returns>The configured brush.</returns>
    private static SolidColorBrush CreateBrush(Color color) => new(color);

    /// <summary>Builds the dashboard header.</summary>
    /// <returns>The header element.</returns>
    private static StackPanel BuildHeader()
    {
        var stack = new StackPanel
        {
            Padding = new(18, 16, 18, 10),
            Spacing = 4
        };

        stack.Children.Add(CreateText("ReactiveUI.Uno.Reactive IoT Operations", 24, FontWeights.Bold));
        stack.Children.Add(CreateText("Live local telemetry, reactive commands, activation, OAPH, interactions, and Uno bindings", 13, FontWeights.Normal));

        return stack;
    }

    /// <summary>Adds a button to a grid location.</summary>
    /// <param name="grid">The target grid.</param>
    /// <param name="button">The button to add.</param>
    /// <param name="row">The target row.</param>
    /// <param name="column">The target column.</param>
    private static void AddButton(Grid grid, Button button, int row, int column)
    {
        Grid.SetRow(button, row);
        Grid.SetColumn(button, column);
        grid.Children.Add(button);
    }

    /// <summary>Builds a titled dashboard panel.</summary>
    /// <param name="title">The panel title.</param>
    /// <param name="children">The panel children.</param>
    /// <returns>The configured panel.</returns>
    private static Border BuildPanel(string title, params UIElement[] children)
    {
        var stack = new StackPanel
        {
            Spacing = 8
        };
        stack.Children.Add(CreateText(title, 18, FontWeights.SemiBold));
        foreach (var child in children)
        {
            stack.Children.Add(child);
        }

        return new()
        {
            Background = CreateBrush(Color.FromArgb(255, 17, 24, 39)),
            BorderBrush = CreateBrush(Color.FromArgb(255, 31, 41, 55)),
            BorderThickness = new(1),
            CornerRadius = new(8),
            Padding = new(14),
            Child = stack
        };
    }

    /// <summary>Builds the dashboard layout.</summary>
    private void BuildLayout()
    {
        var root = new Grid
        {
            Background = CreateBrush(Color.FromArgb(255, 15, 23, 42))
        };
        root.RowDefinitions.Add(new() { Height = GridLength.Auto });
        root.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Star) });
        root.RowDefinitions.Add(new() { Height = GridLength.Auto });

        var header = BuildHeader();
        Grid.SetRow(header, 0);
        root.Children.Add(header);

        var content = BuildContent();
        Grid.SetRow(content, 1);
        root.Children.Add(content);

        var footer = new Border
        {
            Background = CreateBrush(Color.FromArgb(255, 17, 24, 39)),
            Padding = new(14),
            Child = StatusMessageText
        };
        Grid.SetRow(footer, 2);
        root.Children.Add(footer);

        Content = root;
    }

    /// <summary>Builds the dashboard content area.</summary>
    /// <returns>The content element.</returns>
    private Grid BuildContent()
    {
        var content = new Grid
        {
            Padding = new(18),
            ColumnSpacing = 16
        };
        content.ColumnDefinitions.Add(new() { Width = new(2, GridUnitType.Star) });
        content.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star) });

        ConfigureDeviceGrid();
        var scrollViewer = new ScrollViewer
        {
            Content = DeviceGrid
        };
        Grid.SetColumn(scrollViewer, 0);
        content.Children.Add(scrollViewer);

        var sidePanel = BuildSidePanel();
        Grid.SetColumn(sidePanel, 1);
        content.Children.Add(sidePanel);

        return content;
    }

    /// <summary>Configures the device grid dimensions.</summary>
    private void ConfigureDeviceGrid()
    {
        for (var column = 0; column < 3; column++)
        {
            DeviceGrid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star) });
        }

        for (var row = 0; row < 2; row++)
        {
            DeviceGrid.RowDefinitions.Add(new() { Height = GridLength.Auto });
        }
    }

    /// <summary>Builds the dashboard side panel.</summary>
    /// <returns>The side panel element.</returns>
    private StackPanel BuildSidePanel()
    {
        var stack = new StackPanel
        {
            Spacing = 12
        };

        stack.Children.Add(BuildPanel("Stream", StreamStateText, SampleCountText, LastUpdatedText));
        stack.Children.Add(BuildPanel("Selection", SelectedDeviceText, SearchBox, FilterSummaryText));
        stack.Children.Add(BuildCommandPanel());
        stack.Children.Add(BuildPanel("Alerts", LatestAlertText, AlertList, InteractionMessageText));

        return stack;
    }

    /// <summary>Builds the command panel.</summary>
    /// <returns>The command panel element.</returns>
    private Border BuildCommandPanel()
    {
        var buttonGrid = new Grid
        {
            ColumnSpacing = 8,
            RowSpacing = 8
        };
        buttonGrid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star) });
        buttonGrid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star) });
        buttonGrid.RowDefinitions.Add(new() { Height = GridLength.Auto });
        buttonGrid.RowDefinitions.Add(new() { Height = GridLength.Auto });

        AddButton(buttonGrid, ToggleStreamButton, 0, 0);
        AddButton(buttonGrid, RefreshButton, 0, 1);
        AddButton(buttonGrid, AcknowledgeButton, 1, 0);
        AddButton(buttonGrid, ResetButton, 1, 1);

        return BuildPanel("Commands", buttonGrid);
    }

    /// <summary>Creates device-card controls for all devices.</summary>
    /// <param name="viewModel">The dashboard view model.</param>
    private void EnsureDeviceCards(DashboardViewModel viewModel)
    {
        if (_deviceCardsCreated)
        {
            return;
        }

        _deviceCardsCreated = true;
        for (var index = 0; index < viewModel.Devices.Count; index++)
        {
            var device = viewModel.Devices[index];
            var card = new DeviceCardView
            {
                ViewModel = device
            };
            card.SelectButton.Command = viewModel.SelectDevice;
            card.SelectButton.CommandParameter = device;

            Grid.SetRow(card, index / 3);
            Grid.SetColumn(card, index % 3);
            DeviceGrid.Children.Add(card);
        }
    }
}
