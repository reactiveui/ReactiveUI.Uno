// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Text;
using Windows.UI;

namespace ReactiveUI.Uno.Reactive.IoTDashboard.Views;

/// <summary>Displays one live IoT device tile.</summary>
public sealed partial class DeviceCardView : DeviceCardViewBase
{
    /// <summary>Stores the opaque color alpha value.</summary>
    private const byte OpaqueAlpha = 255;

    /// <summary>Stores the card background red channel.</summary>
    private const byte CardBackgroundRed = 17;

    /// <summary>Stores the card background green channel.</summary>
    private const byte CardBackgroundGreen = 24;

    /// <summary>Stores the card background blue channel.</summary>
    private const byte CardBackgroundBlue = 39;

    /// <summary>Stores the card border red channel.</summary>
    private const byte CardBorderRed = 55;

    /// <summary>Stores the card border green channel.</summary>
    private const byte CardBorderGreen = 65;

    /// <summary>Stores the card border blue channel.</summary>
    private const byte CardBorderBlue = 81;

    /// <summary>Stores compact card spacing.</summary>
    private const double CompactSpacing = 6;

    /// <summary>Stores standard card spacing.</summary>
    private const double StandardSpacing = 12;

    /// <summary>Stores the card corner radius.</summary>
    private const double CardCornerRadius = 8;

    /// <summary>Stores the card padding.</summary>
    private const double CardPadding = 16;

    /// <summary>Stores the title font size.</summary>
    private const double TitleFontSize = 17;

    /// <summary>Stores the value font size.</summary>
    private const double ValueFontSize = 28;

    /// <summary>Stores the body font size.</summary>
    private const double BodyFontSize = 13;

    /// <summary>Stores the metadata font size.</summary>
    private const double MetadataFontSize = 12;

    /// <summary>Stores the primary column index.</summary>
    private const int PrimaryColumn = 0;

    /// <summary>Stores the secondary column index.</summary>
    private const int SecondaryColumn = 1;

    /// <summary>Stores the current view binding scope.</summary>
    private CompositeDisposable? _bindings;

    /// <summary>Initializes a new instance of the <see cref="DeviceCardView"/> class.</summary>
    public DeviceCardView()
    {
        BuildLayout();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>Gets the button used to select the device.</summary>
    public Button SelectButton { get; } = new() { Content = "Inspect", HorizontalAlignment = HorizontalAlignment.Right, Margin = new(0, StandardSpacing, 0, 0) };

    /// <summary>Gets the card root.</summary>
    private Border CardRoot { get; } = new()
    {
        Background = CreateBrush(CreateOpaqueColor(CardBackgroundRed, CardBackgroundGreen, CardBackgroundBlue)),
        BorderBrush = CreateBrush(CreateOpaqueColor(CardBorderRed, CardBorderGreen, CardBorderBlue)),
        BorderThickness = new(SecondaryColumn),
        CornerRadius = new(CardCornerRadius),
        Padding = new(CardPadding),
        Margin = new(0, 0, StandardSpacing, StandardSpacing)
    };

    /// <summary>Gets the device name text.</summary>
    private TextBlock NameText { get; } = CreateText("Device", TitleFontSize, FontWeights.SemiBold);

    /// <summary>Gets the value text.</summary>
    private TextBlock ValueText { get; } = CreateText("0", ValueFontSize, FontWeights.Bold);

    /// <summary>Gets the status text.</summary>
    private TextBlock StatusText { get; } = CreateText("Nominal", BodyFontSize, FontWeights.SemiBold);

    /// <summary>Gets the trend text.</summary>
    private TextBlock TrendText { get; } = CreateText("0.0", BodyFontSize, FontWeights.Normal);

    /// <summary>Gets the timestamp text.</summary>
    private TextBlock TimestampText { get; } = CreateText("--:--:--", MetadataFontSize, FontWeights.Normal);

    /// <summary>Gets the selected marker text.</summary>
    private TextBlock SelectionText { get; } = CreateText("MONITORING", MetadataFontSize, FontWeights.SemiBold);

    /// <summary>Creates a text block with dashboard styling.</summary>
    /// <param name="text">The initial text.</param>
    /// <param name="fontSize">The font size.</param>
    /// <param name="fontWeight">The font weight.</param>
    /// <returns>The configured text block.</returns>
    private static TextBlock CreateText(string text, double fontSize, Windows.UI.Text.FontWeight fontWeight) =>
        new() { Text = text, FontSize = fontSize, FontWeight = fontWeight, Foreground = CreateBrush(Microsoft.UI.Colors.White) };

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

    /// <summary>Builds the card layout.</summary>
    private void BuildLayout()
    {
        var stack = new StackPanel { Spacing = CompactSpacing };

        var header = new Grid();
        header.ColumnDefinitions.Add(new() { Width = new(SecondaryColumn, GridUnitType.Star) });
        header.ColumnDefinitions.Add(new() { Width = GridLength.Auto });

        Grid.SetColumn(NameText, PrimaryColumn);
        header.Children.Add(NameText);

        Grid.SetColumn(SelectionText, SecondaryColumn);
        header.Children.Add(SelectionText);

        var detail = new Grid();
        detail.ColumnDefinitions.Add(new() { Width = new(SecondaryColumn, GridUnitType.Star) });
        detail.ColumnDefinitions.Add(new() { Width = GridLength.Auto });

        Grid.SetColumn(StatusText, PrimaryColumn);
        detail.Children.Add(StatusText);

        Grid.SetColumn(TrendText, SecondaryColumn);
        detail.Children.Add(TrendText);

        stack.Children.Add(header);
        stack.Children.Add(ValueText);
        stack.Children.Add(detail);
        stack.Children.Add(TimestampText);
        stack.Children.Add(SelectButton);

        CardRoot.Child = stack;
        Content = CardRoot;
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

        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.DisplayName,
            static view => view.NameText.Text));
        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.ValueText,
            static view => view.ValueText.Text));
        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.StatusText,
            static view => view.StatusText.Text));
        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.TrendText,
            static view => view.TrendText.Text));
        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.TimestampText,
            static view => view.TimestampText.Text));
        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.SelectionText,
            static view => view.SelectionText.Text));
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
