// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Text;
using Windows.UI;

namespace ReactiveUI.Uno.Reactive.IoTDashboard.Views;

/// <summary>Displays one live IoT device tile.</summary>
public sealed partial class DeviceCardView : DeviceCardViewBase
{
    /// <summary>Initializes a new instance of the <see cref="DeviceCardView"/> class.</summary>
    public DeviceCardView()
    {
        BuildLayout();

        _ = this.WhenActivated(disposables =>
        {
            disposables(this.OneWayBind(ViewModel, static vm => vm.DisplayName, static view => view.NameText.Text));
            disposables(this.OneWayBind(ViewModel, static vm => vm.ValueText, static view => view.ValueText.Text));
            disposables(this.OneWayBind(ViewModel, static vm => vm.StatusText, static view => view.StatusText.Text));
            disposables(this.OneWayBind(ViewModel, static vm => vm.TrendText, static view => view.TrendText.Text));
            disposables(this.OneWayBind(ViewModel, static vm => vm.TimestampText, static view => view.TimestampText.Text));
            disposables(this.OneWayBind(ViewModel, static vm => vm.SelectionText, static view => view.SelectionText.Text));
        });
    }

    /// <summary>Gets the button used to select the device.</summary>
    public Button SelectButton { get; } = new()
    {
        Content = "Inspect",
        HorizontalAlignment = HorizontalAlignment.Right,
        Margin = new(0, 12, 0, 0)
    };

    /// <summary>Gets the card root.</summary>
    private Border CardRoot { get; } = new()
    {
        Background = CreateBrush(Color.FromArgb(255, 17, 24, 39)),
        BorderBrush = CreateBrush(Color.FromArgb(255, 55, 65, 81)),
        BorderThickness = new(1),
        CornerRadius = new(8),
        Padding = new(16),
        Margin = new(0, 0, 12, 12)
    };

    /// <summary>Gets the device name text.</summary>
    private TextBlock NameText { get; } = CreateText("Device", 17, FontWeights.SemiBold);

    /// <summary>Gets the value text.</summary>
    private TextBlock ValueText { get; } = CreateText("0", 28, FontWeights.Bold);

    /// <summary>Gets the status text.</summary>
    private TextBlock StatusText { get; } = CreateText("Nominal", 13, FontWeights.SemiBold);

    /// <summary>Gets the trend text.</summary>
    private TextBlock TrendText { get; } = CreateText("0.0", 13, FontWeights.Normal);

    /// <summary>Gets the timestamp text.</summary>
    private TextBlock TimestampText { get; } = CreateText("--:--:--", 12, FontWeights.Normal);

    /// <summary>Gets the selected marker text.</summary>
    private TextBlock SelectionText { get; } = CreateText("MONITORING", 12, FontWeights.SemiBold);

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
            Foreground = CreateBrush(Microsoft.UI.Colors.White)
        };

    /// <summary>Creates a solid color brush.</summary>
    /// <param name="color">The color to apply.</param>
    /// <returns>The configured brush.</returns>
    private static SolidColorBrush CreateBrush(Color color) => new(color);

    /// <summary>Builds the card layout.</summary>
    private void BuildLayout()
    {
        var stack = new StackPanel
        {
            Spacing = 6
        };

        var header = new Grid();
        header.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star) });
        header.ColumnDefinitions.Add(new() { Width = GridLength.Auto });

        Grid.SetColumn(NameText, 0);
        header.Children.Add(NameText);

        Grid.SetColumn(SelectionText, 1);
        header.Children.Add(SelectionText);

        var detail = new Grid();
        detail.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star) });
        detail.ColumnDefinitions.Add(new() { Width = GridLength.Auto });

        Grid.SetColumn(StatusText, 0);
        detail.Children.Add(StatusText);

        Grid.SetColumn(TrendText, 1);
        detail.Children.Add(TrendText);

        stack.Children.Add(header);
        stack.Children.Add(ValueText);
        stack.Children.Add(detail);
        stack.Children.Add(TimestampText);
        stack.Children.Add(SelectButton);

        CardRoot.Child = stack;
        Content = CardRoot;
    }
}
