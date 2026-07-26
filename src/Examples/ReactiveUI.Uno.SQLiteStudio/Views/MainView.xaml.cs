// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
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
    /// <summary>Stores the minimum SQL editor height.</summary>
    private const double QueryEditorMinimumHeight = 100;

    /// <summary>Stores the standard spacing used in layout padding.</summary>
    private const double StandardSpacing = 16;

    /// <summary>Stores compact spacing for header layout.</summary>
    private const double CompactSpacing = 12;

    /// <summary>Stores tight title bottom spacing.</summary>
    private const double TitleBottomSpacing = 4;

    /// <summary>Stores the header title font size.</summary>
    private const double HeaderTitleFontSize = 18;

    /// <summary>Stores the status row index.</summary>
    private const int StatusRowIndex = 2;

    /// <summary>Stores the current view binding scope.</summary>
    private CompositeDisposable? _bindings;

    /// <summary>Initializes a new instance of the <see cref="MainView"/> class.</summary>
    public MainView()
    {
        BuildLayout();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>Gets the SQL editor text box.</summary>
    private TextBox QueryEditor { get; } = new()
    {
        MinHeight = QueryEditorMinimumHeight,
        AcceptsReturn = true,
        TextWrapping = TextWrapping.Wrap,
        Background = CreateBrush(Microsoft.UI.Colors.White),
        BorderBrush = CreateBrush(Microsoft.UI.Colors.Gainsboro),
        Foreground = CreateBrush(Microsoft.UI.Colors.Black)
    };

    /// <summary>Gets the command button that executes the current SQL query.</summary>
    private AppBarButton ExecuteButton { get; } = new() { Icon = new SymbolIcon(Symbol.Play), Label = "Run" };

    /// <summary>Gets the command button that exports the current result set to CSV.</summary>
    private AppBarButton ExportButton { get; } = new() { Icon = new SymbolIcon(Symbol.Save), Label = "Export CSV" };

    /// <summary>Gets the command button that lists database tables.</summary>
    private AppBarButton ListTablesButton { get; } = new() { Icon = new SymbolIcon(Symbol.List), Label = "List Tables" };

    /// <summary>Gets the command button that creates the sample users table.</summary>
    private AppBarButton CreateUsersButton { get; } = new() { Icon = new SymbolIcon(Symbol.Add), Label = "Create Users" };

    /// <summary>Gets the command button that drops the sample users table.</summary>
    private AppBarButton DropUsersButton { get; } = new() { Icon = new SymbolIcon(Symbol.Delete), Label = "Drop Users" };

    /// <summary>Gets the command button that selects the sample SELECT statement.</summary>
    private AppBarButton SampleSelectButton { get; } = new() { Icon = new SymbolIcon(Symbol.Find), Label = "Sample SELECT" };

    /// <summary>Gets the command button that inserts a sample user row.</summary>
    private AppBarButton SampleInsertButton { get; } = new() { Icon = new SymbolIcon(Symbol.AddFriend), Label = "Sample INSERT" };

    /// <summary>Gets the command button that deletes the sample inserted row.</summary>
    private AppBarButton SampleDeleteButton { get; } = new() { Icon = new SymbolIcon(Symbol.Delete), Label = "Sample DELETE" };

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
    private TextBlock StatusText { get; } = new() { Padding = new(CompactSpacing), Foreground = CreateBrush(Microsoft.UI.Colors.Black) };

    /// <summary>Creates a solid color brush for code-built WinUI elements.</summary>
    /// <param name="color">The color to apply.</param>
    /// <returns>The configured brush.</returns>
    private static SolidColorBrush CreateBrush(Color color) => new(color);

    /// <summary>Builds the view layout using WinUI controls.</summary>
    private void BuildLayout()
    {
        var root = new Grid { Background = CreateBrush(Microsoft.UI.Colors.WhiteSmoke) };
        root.RowDefinitions.Add(new() { Height = GridLength.Auto });
        root.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Star) });
        root.RowDefinitions.Add(new() { Height = GridLength.Auto });

        var header = BuildHeader();
        Grid.SetRow(header, 0);
        root.Children.Add(header);

        var content = BuildContent();
        Grid.SetRow(content, 1);
        root.Children.Add(content);

        var status = new Border { Background = CreateBrush(Microsoft.UI.Colors.Gainsboro), Child = StatusText };
        Grid.SetRow(status, StatusRowIndex);
        root.Children.Add(status);

        Content = root;
    }

    /// <summary>Builds the application header and command bar.</summary>
    /// <returns>The header layout.</returns>
    private Grid BuildHeader()
    {
        var header = new Grid { Background = CreateBrush(Microsoft.UI.Colors.DarkSlateGray) };
        header.RowDefinitions.Add(new() { Height = GridLength.Auto });
        header.RowDefinitions.Add(new() { Height = GridLength.Auto });

        var title = new TextBlock
        {
            Margin = new(StandardSpacing, CompactSpacing, StandardSpacing, TitleBottomSpacing),
            FontSize = HeaderTitleFontSize,
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
        var content = new Grid { Padding = new(StandardSpacing), RowSpacing = CompactSpacing };
        content.RowDefinitions.Add(new() { Height = GridLength.Auto });
        content.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Star) });

        Grid.SetRow(QueryEditor, 0);
        content.Children.Add(QueryEditor);

        Grid.SetRow(ResultsViewer, 1);
        content.Children.Add(ResultsViewer);

        return content;
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
        RegisterBindings(disposables);
    }

    /// <summary>Registers ReactiveUI bindings into the supplied disposable scope.</summary>
    /// <param name="disposables">The binding scope.</param>
    private void RegisterBindings(CompositeDisposable disposables)
    {
        disposables.Add(this.Bind(
            ViewModel,
            static vm => vm.QueryText,
            static view => view.QueryEditor.Text));
        disposables.Add(this.BindCommand(
            ViewModel,
            static vm => vm.ExecuteQuery,
            static view => view.ExecuteButton));
        disposables.Add(this.BindCommand(
            ViewModel,
            static vm => vm.ExportCsv,
            static view => view.ExportButton));
        disposables.Add(this.BindCommand(
            ViewModel,
            static vm => vm.ListTables,
            static view => view.ListTablesButton));
        disposables.Add(this.BindCommand(
            ViewModel,
            static vm => vm.CreateUsersTable,
            static view => view.CreateUsersButton));
        disposables.Add(this.BindCommand(
            ViewModel,
            static vm => vm.DropUsersTable,
            static view => view.DropUsersButton));
        disposables.Add(this.BindCommand(
            ViewModel,
            static vm => vm.SampleSelect,
            static view => view.SampleSelectButton));
        disposables.Add(this.BindCommand(
            ViewModel,
            static vm => vm.SampleInsert,
            static view => view.SampleInsertButton));
        disposables.Add(this.BindCommand(
            ViewModel,
            static vm => vm.SampleDelete,
            static view => view.SampleDeleteButton));
        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.ResultsText,
            static view => view.ResultsViewer.Text));
        disposables.Add(this.OneWayBind(
            ViewModel,
            static vm => vm.Status,
            static view => view.StatusText.Text));
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
