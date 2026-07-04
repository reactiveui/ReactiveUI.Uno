// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace ReactiveUI.Uno.SQLiteStudio.Views;

/// <summary>
/// Represents the main user interface view for the application, providing controls for query editing, execution, and
/// data management.
/// </summary>
/// <remarks>MainView binds UI elements to the associated view model, enabling users to compose and execute
/// queries, export results, and manage database tables.</remarks>
public sealed partial class MainView : MainViewBase
{
    /// <summary>Initializes a new instance of the <see cref="MainView"/> class.</summary>
    public MainView()
    {
        BuildLayout();

        _ = this.WhenActivated((disposables) =>
        {
            disposables(this.Bind(ViewModel, vm => vm.QueryText, v => v.QueryEditor.Text));
            disposables(this.BindCommand(ViewModel, vm => vm.ExecuteQuery, v => v.ExecuteButton));
            disposables(this.BindCommand(ViewModel, vm => vm.ExportCsv, v => v.ExportButton));
            disposables(this.BindCommand(ViewModel, vm => vm.ListTables, v => v.ListTablesButton));
            disposables(this.BindCommand(ViewModel, vm => vm.CreateUsersTable, v => v.CreateUsersButton));
            disposables(this.BindCommand(ViewModel, vm => vm.DropUsersTable, v => v.DropUsersButton));
            disposables(this.BindCommand(ViewModel, vm => vm.SampleSelect, v => v.SampleSelectButton));
            disposables(this.BindCommand(ViewModel, vm => vm.SampleInsert, v => v.SampleInsertButton));
            disposables(this.BindCommand(ViewModel, vm => vm.SampleDelete, v => v.SampleDeleteButton));
            disposables(this.OneWayBind(ViewModel, vm => vm.ResultsText, v => v.ResultsViewer.Text));
            disposables(this.OneWayBind(ViewModel, vm => vm.Status, v => v.StatusText.Text));
        });
    }

    /// <summary>Gets the SQL editor text box.</summary>
    private TextBox QueryEditor { get; } = new()
    {
        MinHeight = 100,
        AcceptsReturn = true,
        TextWrapping = TextWrapping.Wrap,
        Background = CreateBrush(Microsoft.UI.Colors.White),
        BorderBrush = CreateBrush(Microsoft.UI.Colors.Gainsboro),
        Foreground = CreateBrush(Microsoft.UI.Colors.Black)
    };

    /// <summary>Gets the command button that executes the current SQL query.</summary>
    private AppBarButton ExecuteButton { get; } = new()
    {
        Icon = new SymbolIcon(Symbol.Play),
        Label = "Run"
    };

    /// <summary>Gets the command button that exports the current result set to CSV.</summary>
    private AppBarButton ExportButton { get; } = new()
    {
        Icon = new SymbolIcon(Symbol.Save),
        Label = "Export CSV"
    };

    /// <summary>Gets the command button that lists database tables.</summary>
    private AppBarButton ListTablesButton { get; } = new()
    {
        Icon = new SymbolIcon(Symbol.List),
        Label = "List Tables"
    };

    /// <summary>Gets the command button that creates the sample users table.</summary>
    private AppBarButton CreateUsersButton { get; } = new()
    {
        Icon = new SymbolIcon(Symbol.Add),
        Label = "Create Users"
    };

    /// <summary>Gets the command button that drops the sample users table.</summary>
    private AppBarButton DropUsersButton { get; } = new()
    {
        Icon = new SymbolIcon(Symbol.Delete),
        Label = "Drop Users"
    };

    /// <summary>Gets the command button that selects the sample SELECT statement.</summary>
    private AppBarButton SampleSelectButton { get; } = new()
    {
        Icon = new SymbolIcon(Symbol.Find),
        Label = "Sample SELECT"
    };

    /// <summary>Gets the command button that inserts a sample user row.</summary>
    private AppBarButton SampleInsertButton { get; } = new()
    {
        Icon = new SymbolIcon(Symbol.AddFriend),
        Label = "Sample INSERT"
    };

    /// <summary>Gets the command button that deletes the sample inserted row.</summary>
    private AppBarButton SampleDeleteButton { get; } = new()
    {
        Icon = new SymbolIcon(Symbol.Delete),
        Label = "Sample DELETE"
    };

    /// <summary>Gets the read-only text box that displays query results.</summary>
    private TextBox ResultsViewer { get; } = new()
    {
        AcceptsReturn = true,
        Background = CreateBrush(Microsoft.UI.Colors.White),
        BorderBrush = CreateBrush(Microsoft.UI.Colors.Gainsboro),
        BorderThickness = new(1),
        Foreground = CreateBrush(Microsoft.UI.Colors.Black),
        IsReadOnly = true,
        TextWrapping = TextWrapping.Wrap
    };

    /// <summary>Gets the status text displayed at the bottom of the view.</summary>
    private TextBlock StatusText { get; } = new()
    {
        Padding = new(12),
        Foreground = CreateBrush(Microsoft.UI.Colors.Black)
    };

    /// <summary>Creates a solid color brush for code-built WinUI elements.</summary>
    /// <param name="color">The color to apply.</param>
    /// <returns>The configured brush.</returns>
    private static SolidColorBrush CreateBrush(Color color) => new(color);

    /// <summary>Builds the view layout using WinUI controls.</summary>
    private void BuildLayout()
    {
        var root = new Grid
        {
            Background = CreateBrush(Microsoft.UI.Colors.WhiteSmoke)
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

        var status = new Border
        {
            Background = CreateBrush(Microsoft.UI.Colors.Gainsboro),
            Child = StatusText
        };
        Grid.SetRow(status, 2);
        root.Children.Add(status);

        Content = root;
    }

    /// <summary>Builds the application header and command bar.</summary>
    /// <returns>The header layout.</returns>
    private Grid BuildHeader()
    {
        var header = new Grid
        {
            Background = CreateBrush(Microsoft.UI.Colors.DarkSlateGray)
        };
        header.RowDefinitions.Add(new() { Height = GridLength.Auto });
        header.RowDefinitions.Add(new() { Height = GridLength.Auto });

        var title = new TextBlock
        {
            Margin = new(16, 12, 16, 4),
            FontSize = 18,
            FontWeight = FontWeights.SemiBold,
            Foreground = CreateBrush(Microsoft.UI.Colors.White),
            Text = "ReactiveUI.Uno SQLite Studio"
        };
        header.Children.Add(title);

        var commandBar = new CommandBar
        {
            Background = CreateBrush(Microsoft.UI.Colors.DarkSlateGray),
            DefaultLabelPosition = CommandBarDefaultLabelPosition.Right,
            Foreground = CreateBrush(Microsoft.UI.Colors.White)
        };

        commandBar.PrimaryCommands.Add(ExecuteButton);
        commandBar.PrimaryCommands.Add(ExportButton);
        commandBar.PrimaryCommands.Add(new AppBarSeparator());
        commandBar.PrimaryCommands.Add(ListTablesButton);
        commandBar.PrimaryCommands.Add(CreateUsersButton);
        commandBar.PrimaryCommands.Add(DropUsersButton);
        commandBar.PrimaryCommands.Add(new AppBarSeparator());
        commandBar.PrimaryCommands.Add(SampleSelectButton);
        commandBar.PrimaryCommands.Add(SampleInsertButton);
        commandBar.PrimaryCommands.Add(SampleDeleteButton);

        Grid.SetRow(commandBar, 1);
        header.Children.Add(commandBar);

        return header;
    }

    /// <summary>Builds the query editor and results region.</summary>
    /// <returns>The content layout.</returns>
    private Grid BuildContent()
    {
        var content = new Grid
        {
            Padding = new(16),
            RowSpacing = 12
        };
        content.RowDefinitions.Add(new() { Height = GridLength.Auto });
        content.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Star) });

        Grid.SetRow(QueryEditor, 0);
        content.Children.Add(QueryEditor);

        Grid.SetRow(ResultsViewer, 1);
        content.Children.Add(ResultsViewer);

        return content;
    }
}
