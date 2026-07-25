// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Text;
using ReactiveUI.Uno.Reactive.IoTDashboard.Models;
using ReactiveUI.Uno.Reactive.IoTDashboard.ViewModels;
using Windows.UI;

namespace ReactiveUI.Uno.Reactive.IoTDashboard.Views;

/// <summary>Displays the live IoT dashboard sample.</summary>
public sealed partial class DashboardView : DashboardViewBase
{
    /// <summary>Stores the opaque color alpha value.</summary>
    private const byte OpaqueAlpha = 255;

    /// <summary>Stores the page background red channel.</summary>
    private const byte PageBackgroundRed = 15;

    /// <summary>Stores the page background green channel.</summary>
    private const byte PageBackgroundGreen = 23;

    /// <summary>Stores the page background blue channel.</summary>
    private const byte PageBackgroundBlue = 42;

    /// <summary>Stores the panel background red channel.</summary>
    private const byte PanelBackgroundRed = 17;

    /// <summary>Stores the panel background green channel.</summary>
    private const byte PanelBackgroundGreen = 24;

    /// <summary>Stores the panel background blue channel.</summary>
    private const byte PanelBackgroundBlue = 39;

    /// <summary>Stores the panel border red channel.</summary>
    private const byte PanelBorderRed = 31;

    /// <summary>Stores the panel border green channel.</summary>
    private const byte PanelBorderGreen = 41;

    /// <summary>Stores the panel border blue channel.</summary>
    private const byte PanelBorderBlue = 55;

    /// <summary>Stores compact layout spacing.</summary>
    private const double CompactSpacing = 4;

    /// <summary>Stores standard layout spacing.</summary>
    private const double StandardSpacing = 8;

    /// <summary>Stores panel stack spacing.</summary>
    private const double PanelStackSpacing = 12;

    /// <summary>Stores the page padding.</summary>
    private const double PagePadding = 18;

    /// <summary>Stores the header top padding.</summary>
    private const double HeaderTopPadding = 16;

    /// <summary>Stores the header bottom padding.</summary>
    private const double HeaderBottomPadding = 10;

    /// <summary>Stores the panel padding.</summary>
    private const double PanelPadding = 14;

    /// <summary>Stores the alert list maximum height.</summary>
    private const double AlertListMaximumHeight = 160;

    /// <summary>Stores the headline font size.</summary>
    private const double HeadlineFontSize = 24;

    /// <summary>Stores the panel title font size.</summary>
    private const double PanelTitleFontSize = 18;

    /// <summary>Stores the prominent value font size.</summary>
    private const double ProminentFontSize = 16;

    /// <summary>Stores the regular body font size.</summary>
    private const double BodyFontSize = 15;

    /// <summary>Stores the compact body font size.</summary>
    private const double CompactFontSize = 13;

    /// <summary>Stores the primary content column ratio.</summary>
    private const double PrimaryContentColumnRatio = 2;

    /// <summary>Stores the secondary content column ratio.</summary>
    private const double SecondaryContentColumnRatio = 1;

    /// <summary>Stores the device grid column count.</summary>
    private const int DeviceGridColumnCount = 3;

    /// <summary>Stores the device grid row count.</summary>
    private const int DeviceGridRowCount = 2;

    /// <summary>Stores the header row index.</summary>
    private const int HeaderRow = 0;

    /// <summary>Stores the content row index.</summary>
    private const int ContentRow = 1;

    /// <summary>Stores the footer row index.</summary>
    private const int FooterRow = 2;

    /// <summary>Stores the left command column index.</summary>
    private const int LeftCommandColumn = 0;

    /// <summary>Stores the right command column index.</summary>
    private const int RightCommandColumn = 1;

    /// <summary>Stores the top command row index.</summary>
    private const int TopCommandRow = 0;

    /// <summary>Stores the bottom command row index.</summary>
    private const int BottomCommandRow = 1;

    /// <summary>Tracks whether device cards have been created.</summary>
    private bool _deviceCardsCreated;

    /// <summary>Stores the current view binding scope.</summary>
    private CompositeDisposable? _bindings;

    /// <summary>Initializes a new instance of the <see cref="DashboardView"/> class.</summary>
    public DashboardView()
    {
        BuildLayout();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>Gets the grid that hosts device cards.</summary>
    private Grid DeviceGrid { get; } = new() { ColumnSpacing = StandardSpacing, RowSpacing = StandardSpacing };

    /// <summary>Gets the alert list.</summary>
    private ListView AlertList { get; } = new() { MaxHeight = AlertListMaximumHeight };

    /// <summary>Gets the search text box.</summary>
    private TextBox SearchBox { get; } = new() { PlaceholderText = "Filter devices", Margin = new(0, CompactSpacing, 0, 0) };

    /// <summary>Gets the stream state text.</summary>
    private TextBlock StreamStateText { get; } =
        CreateText("Live stream running", ProminentFontSize, FontWeights.SemiBold);

    /// <summary>Gets the sample count text.</summary>
    private TextBlock SampleCountText { get; } =
        CreateText("0 samples processed", ProminentFontSize, FontWeights.SemiBold);

    /// <summary>Gets the selected device text.</summary>
    private TextBlock SelectedDeviceText { get; } =
        CreateText("Select a device", BodyFontSize, FontWeights.Normal);

    /// <summary>Gets the latest update text.</summary>
    private TextBlock LastUpdatedText { get; } =
        CreateText("Last update --:--:--", BodyFontSize, FontWeights.Normal);

    /// <summary>Gets the filter summary text.</summary>
    private TextBlock FilterSummaryText { get; } =
        CreateText("6 devices visible", CompactFontSize, FontWeights.Normal);

    /// <summary>Gets the latest alert text.</summary>
    private TextBlock LatestAlertText { get; } =
        CreateText("No active alerts.", BodyFontSize, FontWeights.SemiBold);

    /// <summary>Gets the interaction message text.</summary>
    private TextBlock InteractionMessageText { get; } =
        CreateText("No operator interaction yet.", CompactFontSize, FontWeights.Normal);

    /// <summary>Gets the status message text.</summary>
    private TextBlock StatusMessageText { get; } = CreateText("Ready", CompactFontSize, FontWeights.Normal);

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
        new() { Text = text, FontSize = fontSize, FontWeight = fontWeight, Foreground = CreateBrush(Microsoft.UI.Colors.White), TextWrapping = TextWrapping.Wrap };

    /// <summary>Creates a solid color brush.</summary>
    /// <param name="color">The color to apply.</param>
    /// <returns>The configured brush.</returns>
    private static SolidColorBrush CreateBrush(Color color) => new(color);

    /// <summary>Creates an opaque color.</summary>
    /// <param name="red">The red channel.</param>
    /// <param name="green">The green channel.</param>
    /// <param name="blue">The blue channel.</param>
    /// <returns>The configured color.</returns>
    private static Color CreateOpaqueColor(byte red, byte green, byte blue) =>
        Color.FromArgb(OpaqueAlpha, red, green, blue);

    /// <summary>Builds the dashboard header.</summary>
    /// <returns>The header element.</returns>
    private static StackPanel BuildHeader()
    {
        var stack = new StackPanel { Padding = new(PagePadding, HeaderTopPadding, PagePadding, HeaderBottomPadding), Spacing = CompactSpacing };

        stack.Children.Add(CreateText(
            "ReactiveUI.Uno.Reactive IoT Operations",
            HeadlineFontSize,
            FontWeights.Bold));
        stack.Children.Add(CreateText(
            "Live local telemetry, reactive commands, activation, OAPH, interactions, and Uno bindings",
            CompactFontSize,
            FontWeights.Normal));

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
        var stack = new StackPanel { Spacing = StandardSpacing };
        stack.Children.Add(CreateText(title, PanelTitleFontSize, FontWeights.SemiBold));
        foreach (var child in children)
        {
            stack.Children.Add(child);
        }

        return new()
        {
            Background = CreateBrush(CreateOpaqueColor(
                PanelBackgroundRed,
                PanelBackgroundGreen,
                PanelBackgroundBlue)),
            BorderBrush = CreateBrush(CreateOpaqueColor(PanelBorderRed, PanelBorderGreen, PanelBorderBlue)),
            BorderThickness = new(SecondaryContentColumnRatio),
            CornerRadius = new(StandardSpacing),
            Padding = new(PanelPadding),
            Child = stack
        };
    }

    /// <summary>Builds the dashboard layout.</summary>
    private void BuildLayout()
    {
        var root = new Grid { Background = CreateBrush(CreateOpaqueColor(PageBackgroundRed, PageBackgroundGreen, PageBackgroundBlue)) };
        root.RowDefinitions.Add(new() { Height = GridLength.Auto });
        root.RowDefinitions.Add(new() { Height = new(SecondaryContentColumnRatio, GridUnitType.Star) });
        root.RowDefinitions.Add(new() { Height = GridLength.Auto });

        var header = BuildHeader();
        Grid.SetRow(header, HeaderRow);
        root.Children.Add(header);

        var content = BuildContent();
        Grid.SetRow(content, ContentRow);
        root.Children.Add(content);

        var footer = new Border { Background = CreateBrush(CreateOpaqueColor(PanelBackgroundRed, PanelBackgroundGreen, PanelBackgroundBlue)), Padding = new(PanelPadding), Child = StatusMessageText };
        Grid.SetRow(footer, FooterRow);
        root.Children.Add(footer);

        Content = root;
    }

    /// <summary>Builds the dashboard content area.</summary>
    /// <returns>The content element.</returns>
    private Grid BuildContent()
    {
        var content = new Grid { Padding = new(PagePadding), ColumnSpacing = HeaderTopPadding };
        content.ColumnDefinitions.Add(new() { Width = new(PrimaryContentColumnRatio, GridUnitType.Star) });
        content.ColumnDefinitions.Add(new() { Width = new(SecondaryContentColumnRatio, GridUnitType.Star) });

        ConfigureDeviceGrid();
        var scrollViewer = new ScrollViewer { Content = DeviceGrid };
        Grid.SetColumn(scrollViewer, LeftCommandColumn);
        content.Children.Add(scrollViewer);

        var sidePanel = BuildSidePanel();
        Grid.SetColumn(sidePanel, RightCommandColumn);
        content.Children.Add(sidePanel);

        return content;
    }

    /// <summary>Configures the device grid dimensions.</summary>
    private void ConfigureDeviceGrid()
    {
        for (var column = 0; column < DeviceGridColumnCount; column++)
        {
            DeviceGrid.ColumnDefinitions.Add(new() { Width = new(SecondaryContentColumnRatio, GridUnitType.Star) });
        }

        for (var row = 0; row < DeviceGridRowCount; row++)
        {
            DeviceGrid.RowDefinitions.Add(new() { Height = GridLength.Auto });
        }
    }

    /// <summary>Builds the dashboard side panel.</summary>
    /// <returns>The side panel element.</returns>
    private StackPanel BuildSidePanel()
    {
        var stack = new StackPanel { Spacing = PanelStackSpacing };

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
        var buttonGrid = new Grid { ColumnSpacing = StandardSpacing, RowSpacing = StandardSpacing };
        buttonGrid.ColumnDefinitions.Add(new() { Width = new(SecondaryContentColumnRatio, GridUnitType.Star) });
        buttonGrid.ColumnDefinitions.Add(new() { Width = new(SecondaryContentColumnRatio, GridUnitType.Star) });
        buttonGrid.RowDefinitions.Add(new() { Height = GridLength.Auto });
        buttonGrid.RowDefinitions.Add(new() { Height = GridLength.Auto });

        AddButton(buttonGrid, ToggleStreamButton, TopCommandRow, LeftCommandColumn);
        AddButton(buttonGrid, RefreshButton, TopCommandRow, RightCommandColumn);
        AddButton(buttonGrid, AcknowledgeButton, BottomCommandRow, LeftCommandColumn);
        AddButton(buttonGrid, ResetButton, BottomCommandRow, RightCommandColumn);

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
            var card = new DeviceCardView { ViewModel = device };
            card.SelectButton.Command = viewModel.SelectDevice;
            card.SelectButton.CommandParameter = device;

            Grid.SetRow(card, index / DeviceGridColumnCount);
            Grid.SetColumn(card, index % DeviceGridColumnCount);
            DeviceGrid.Children.Add(card);
        }
    }

    /// <summary>Creates ReactiveUI bindings when the view enters the visual tree.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(args);

        if (_bindings is not null)
        {
            return;
        }

        var disposables = new CompositeDisposable();
        _bindings = disposables;

        InitializeDashboardData();
        RegisterBindings(disposables);
    }

    /// <summary>Connects dashboard collections to the view when a view model is available.</summary>
    private void InitializeDashboardData()
    {
        if (ViewModel is null)
        {
            return;
        }

        EnsureDeviceCards(ViewModel);
        AlertList.ItemsSource = ViewModel.Alerts;
    }

    /// <summary>Registers ReactiveUI bindings into the supplied disposable scope.</summary>
    /// <param name="disposables">The binding scope.</param>
    private void RegisterBindings(CompositeDisposable disposables)
    {
        disposables.Add(this.Bind(
            ViewModel,
            static vm => vm.SearchText,
            static view => view.SearchBox.Text));
        disposables.Add(this.BindCommand(
            ViewModel,
            static vm => vm.ToggleStreaming,
            static view => view.ToggleStreamButton));
        disposables.Add(this.BindCommand(
            ViewModel,
            static vm => vm.RefreshSnapshot,
            static view => view.RefreshButton));
        disposables.Add(this.BindCommand(
            ViewModel,
            static vm => vm.AcknowledgeAlert,
            static view => view.AcknowledgeButton));
        disposables.Add(this.BindCommand(
            ViewModel,
            static vm => vm.ResetSimulation,
            static view => view.ResetButton));
        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.StreamStateText,
            static view => view.StreamStateText.Text));
        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.SampleCountText,
            static view => view.SampleCountText.Text));
        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.SelectedDeviceSummary,
            static view => view.SelectedDeviceText.Text));
        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.LastUpdatedText,
            static view => view.LastUpdatedText.Text));
        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.FilterSummary,
            static view => view.FilterSummaryText.Text));
        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.LatestAlertText,
            static view => view.LatestAlertText.Text));
        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.InteractionMessage,
            static view => view.InteractionMessageText.Text));
        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.StatusMessage,
            static view => view.StatusMessageText.Text));
        disposables.Add(this.BindInteraction(
            ViewModel,
            static vm => vm.ConfirmAcknowledge,
            HandleAcknowledgeAsync));
    }

    /// <summary>Disposes ReactiveUI bindings when the view leaves the visual tree.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    private void OnUnloaded(object sender, RoutedEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(args);

        _bindings?.Dispose();
        _bindings = null;
    }
}
