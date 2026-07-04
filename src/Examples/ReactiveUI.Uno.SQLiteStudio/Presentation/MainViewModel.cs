// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using ReactiveUI.Uno.SQLiteStudio.Services;
using RxVoid = ReactiveUI.Primitives.RxVoid;

namespace ReactiveUI.Uno.SQLiteStudio.Presentation;

/// <summary>
/// Represents the main view model for the application's primary user interface, providing commands and properties for
/// querying, exporting, and managing data in a reactive MVVM pattern.
/// </summary>
/// <remarks>MainViewModel coordinates database operations, CSV export, and UI state management using reactive
/// commands and observable properties. It is designed for use with ReactiveUI and supports navigation via the
/// IRoutableViewModel interface. Service dependencies are supplied explicitly so the view model remains testable and
/// avoids service-locator fallbacks. This view model is intended for applications that require dynamic data querying
/// and export functionality, and is suitable for binding in reactive user interfaces.</remarks>
public class MainViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>Stores the SQLite database service.</summary>
    private readonly ISqliteService _db;

    /// <summary>Stores the CSV export service.</summary>
    private readonly ICsvExportService _csv;

    /// <summary>Stores the mutable backing collection for query results.</summary>
    private ObservableCollection<object> _resultsBacking = [];

    /// <summary>Initializes a new instance of the <see cref="MainViewModel"/> class with the specified navigation host and service dependencies.</summary>
    /// <remarks>The view model sets up commands for querying, exporting, and managing database tables, and is
    /// intended for use in reactive UI scenarios.</remarks>
    /// <param name="hostScreen">The navigation host screen used for routing and view model location. Cannot be null.</param>
    /// <param name="sqlite">The SQLite service used for database operations.</param>
    /// <param name="csv">The CSV export service used for exporting result data.</param>
    public MainViewModel(IScreen hostScreen, ISqliteService sqlite, ICsvExportService csv)
    {
        ArgumentNullException.ThrowIfNull(hostScreen);
        ArgumentNullException.ThrowIfNull(sqlite);
        ArgumentNullException.ThrowIfNull(csv);

        HostScreen = hostScreen;
        UrlPathSegment = "Main";

        _db = sqlite;
        _csv = csv;

        ExecuteQuery = ReactiveCommand.CreateFromTask(async () =>
        {
            Status = "Running...";
            try
            {
                var result = await _db.QueryAsync(QueryText ?? string.Empty).ConfigureAwait(true);
                UpdateResults(result);
                Status = $"Rows: {_resultsBacking.Count}";
            }
            catch (Exception ex)
            {
                Status = ex.Message;
            }
        });

        ExportCsv = ReactiveCommand.CreateFromTask(
            async () =>
            {
                try
                {
                    await _csv.ExportAsync(_resultsBacking).ConfigureAwait(true);
                    Status = "Exported results.csv";
                }
                catch (Exception ex)
                {
                    Status = ex.Message;
                }
            },
            this.WhenAnyValue(x => x.Results, results => results is { Count: > 0 }));

        ListTables = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                var tables = await _db.ListTablesAsync().ConfigureAwait(true);
                var tableResults = new List<object>(tables.Count);
                tableResults.AddRange(tables);

                UpdateResults(tableResults);
                Status = $"Tables: {_resultsBacking.Count}";
            }
            catch (Exception ex)
            {
                Status = ex.Message;
            }
        });

        CreateUsersTable = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                const string sql = "CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Age INTEGER);";
                await _db.ExecuteAsync(sql).ConfigureAwait(true);
                Status = "Users table ensured.";
            }
            catch (Exception ex)
            {
                Status = ex.Message;
            }
        });

        DropUsersTable = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                await _db.ExecuteAsync("DROP TABLE IF EXISTS Users;").ConfigureAwait(true);
                Status = "Users table dropped.";
            }
            catch (Exception ex)
            {
                Status = ex.Message;
            }
        });

        // Sample scripts
        SampleSelect = ReactiveCommand.Create(SetSampleSelectQuery);

        SampleInsert = ReactiveCommand.CreateFromTask(async () =>
        {
            await _db.ExecuteAsync("INSERT INTO Users (Name, Age) VALUES ('Dave', 28);").ConfigureAwait(true);
            QueryText = "SELECT * FROM Users;";
            await ExecuteQuery.Execute();
        });

        SampleDelete = ReactiveCommand.CreateFromTask(async () =>
        {
            await _db.ExecuteAsync("DELETE FROM Users WHERE Name = 'Dave';").ConfigureAwait(true);
            QueryText = "SELECT * FROM Users;";
            await ExecuteQuery.Execute();
        });
    }

    /// <summary>Gets the URL path segment associated with this resource.</summary>
    public string UrlPathSegment { get; }

    /// <summary>Gets the screen that hosts the current view model.</summary>
    /// <remarks>The host screen is typically used for navigation and view resolution within applications that
    /// implement the IScreen pattern. This property is read-only and is set during the construction of the view
    /// model.</remarks>
    public IScreen HostScreen { get; }

    /// <summary>Gets or sets the text of the query to be executed.</summary>
    public string QueryText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
    = "SELECT * FROM Users;";

    /// <summary>Gets a read-only observable collection containing the current results.</summary>
    public ReadOnlyObservableCollection<object> Results
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }
    = new([]);

    /// <summary>Gets the formatted result output displayed by the sample view.</summary>
    public string ResultsText
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }
    = string.Empty;

    /// <summary>Gets or sets the current status message for the object.</summary>
    public string Status
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
    = "Ready";

    /// <summary>Gets the command that executes the query operation asynchronously.</summary>
    /// <remarks>This command can be bound to UI elements to trigger the query execution. The command's
    /// execution and availability are managed reactively, allowing for integration with reactive UI patterns.</remarks>
    public ReactiveCommand<RxVoid, RxVoid> ExecuteQuery { get; }

    /// <summary>Gets the command that exports the current data to a CSV file.</summary>
    /// <remarks>This command can be bound to a user interface element to trigger CSV export functionality.
    /// The command completes when the export operation finishes.</remarks>
    public ReactiveCommand<RxVoid, RxVoid> ExportCsv { get; }

    /// <summary>Gets the command that initiates retrieval of the list of tables from the data source.</summary>
    /// <remarks>The command can be executed to trigger the process of listing available tables. The command's
    /// execution and availability may be bound to UI elements in reactive user interfaces. This property is typically
    /// used in MVVM scenarios to encapsulate the logic for listing tables.</remarks>
    public ReactiveCommand<RxVoid, RxVoid> ListTables { get; }

    /// <summary>Gets the command that creates the users table in the database asynchronously.</summary>
    /// <remarks>This command can be executed to ensure that the users table exists before performing
    /// operations that depend on it. The command completes when the table creation operation finishes. If the table
    /// already exists, executing the command has no effect.</remarks>
    public ReactiveCommand<RxVoid, RxVoid> CreateUsersTable { get; }

    /// <summary>Gets a command that drops the users table from the database.</summary>
    /// <remarks>Executing this command will permanently remove all user data from the database. Use with
    /// caution, as this action cannot be undone.</remarks>
    public ReactiveCommand<RxVoid, RxVoid> DropUsersTable { get; }

    /// <summary>Gets the command that executes the sample selection action.</summary>
    /// <remarks>Use this command to trigger the selection logic in response to user interaction or other
    /// events. The command completes immediately and does not return a result value.</remarks>
    public ReactiveCommand<RxVoid, RxVoid> SampleSelect { get; }

    /// <summary>Gets a command that inserts a sample item when executed.</summary>
    public ReactiveCommand<RxVoid, RxVoid> SampleInsert { get; }

    /// <summary>Gets the command that deletes the currently selected sample.</summary>
    /// <remarks>This command can be used to trigger the deletion of a sample in the user interface. The
    /// command's ability to execute may depend on whether a sample is currently selected. Use the command's CanExecute
    /// observable to determine if deletion is currently possible.</remarks>
    public ReactiveCommand<RxVoid, RxVoid> SampleDelete { get; }

    /// <summary>Formats result rows for the sample result pane.</summary>
    /// <param name="results">The result rows to format.</param>
    /// <returns>A display string for the result pane.</returns>
    private static string FormatResults(IEnumerable<object> results)
    {
        var rows = results.Select(static item => item?.ToString() ?? string.Empty);
        var text = string.Join(Environment.NewLine, rows);
        return string.IsNullOrWhiteSpace(text) ? "(no rows)" : text;
    }

    /// <summary>Sets the query text to the sample select query.</summary>
    private void SetSampleSelectQuery() => QueryText = "SELECT * FROM Users ORDER BY Name;";

    /// <summary>Updates the collection and text representation of the current results.</summary>
    /// <param name="results">The result rows to publish.</param>
    private void UpdateResults(IReadOnlyList<object> results)
    {
        _resultsBacking = new(results);
        Results = new(_resultsBacking);
        ResultsText = FormatResults(_resultsBacking);
    }
}
