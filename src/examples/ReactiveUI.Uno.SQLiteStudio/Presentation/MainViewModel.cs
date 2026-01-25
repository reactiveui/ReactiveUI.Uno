using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Uno.SQLiteStudio.Services;

namespace ReactiveUI.Uno.SQLiteStudio.Presentation;

/// <summary>
/// Represents the main view model for the application's primary user interface, providing commands and properties for
/// querying, exporting, and managing data in a reactive MVVM pattern.
/// </summary>
/// <remarks>MainViewModel coordinates database operations, CSV export, and UI state management using reactive
/// commands and observable properties. It is designed for use with ReactiveUI and supports navigation via the
/// IRoutableViewModel interface. Service dependencies for database and CSV export can be supplied or resolved via a
/// service locator. This view model is intended for use in applications that require dynamic data querying and export
/// functionality, and is suitable for binding in reactive user interfaces.</remarks>
public partial class MainViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly ISqliteService _db;
    private readonly ICsvExportService _csv;

    private string? _queryText = "SELECT * FROM Users;";
    private ReadOnlyObservableCollection<object>? _results;
    private ObservableCollection<object> _resultsBacking = [];
    private string? _status;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class with the specified navigation host and optional service.
    /// dependencies.
    /// </summary>
    /// <remarks>This constructor uses reflection to resolve default service implementations if the sqlite or
    /// csv parameters are not provided. This may not be compatible with ahead-of-time (AOT) compilation environments.
    /// The view model sets up commands for querying, exporting, and managing database tables, and is intended for use
    /// in reactive UI scenarios.</remarks>
    /// <param name="hostScreen">The navigation host screen used for routing and view model location. Cannot be null.</param>
    /// <param name="sqlite">An optional ISqliteService implementation for database operations. If null, a default service is resolved from
    /// the application service locator.</param>
    /// <param name="csv">An optional ICsvExportService implementation for exporting data to CSV. If null, a default service is resolved
    /// from the application service locator.</param>
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
    public MainViewModel(IScreen hostScreen, ISqliteService? sqlite = null, ICsvExportService? csv = null)
    {
        HostScreen = hostScreen;
        UrlPathSegment = "Main";

        _db = sqlite ?? AppLocator.Current.GetService<ISqliteService>()!;
        _csv = csv ?? AppLocator.Current.GetService<ICsvExportService>()!;

        ExecuteQuery = ReactiveCommand.CreateFromTask(async () =>
        {
            Status = "Running...";
            try
            {
                var result = await _db.QueryAsync(QueryText ?? string.Empty).ConfigureAwait(true);
                _resultsBacking = new ObservableCollection<object>(result);
                _results = new ReadOnlyObservableCollection<object>(_resultsBacking);
                this.RaisePropertyChanged(nameof(Results));
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
            this.WhenAnyValue(x => x.Results).Select(r => r is { Count: > 0 }));

        ListTables = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                var tables = await _db.ListTablesAsync().ConfigureAwait(true);
                _resultsBacking = new ObservableCollection<object>(tables.Cast<object>());
                _results = new ReadOnlyObservableCollection<object>(_resultsBacking);
                this.RaisePropertyChanged(nameof(Results));
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
                var sql = "CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Age INTEGER);";
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
        SampleSelect = ReactiveCommand.Create(() =>
        {
            QueryText = "SELECT * FROM Users ORDER BY Name;";
        });

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

    /// <summary>
    /// Gets the URL path segment associated with this resource.
    /// </summary>
    public string? UrlPathSegment { get; }

    /// <summary>
    /// Gets the screen that hosts the current view model.
    /// </summary>
    /// <remarks>The host screen is typically used for navigation and view resolution within applications that
    /// implement the IScreen pattern. This property is read-only and is set during the construction of the view
    /// model.</remarks>
    public IScreen HostScreen { get; }

    /// <summary>
    /// Gets or sets the text of the query to be executed.
    /// </summary>
    public string? QueryText
    {
        get => _queryText;
        set => this.RaiseAndSetIfChanged(ref _queryText, value);
    }

    /// <summary>
    /// Gets a read-only observable collection containing the current results.
    /// </summary>
    public ReadOnlyObservableCollection<object>? Results => _results;

    /// <summary>
    /// Gets or sets the current status message for the object.
    /// </summary>
    public string? Status
    {
        get => _status;
        set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    /// <summary>
    /// Gets the command that executes the query operation asynchronously.
    /// </summary>
    /// <remarks>This command can be bound to UI elements to trigger the query execution. The command's
    /// execution and availability are managed reactively, allowing for integration with reactive UI patterns.</remarks>
    public ReactiveCommand<Unit, Unit> ExecuteQuery { get; }

    /// <summary>
    /// Gets the command that exports the current data to a CSV file.
    /// </summary>
    /// <remarks>This command can be bound to a user interface element to trigger CSV export functionality.
    /// The command completes when the export operation finishes.</remarks>
    public ReactiveCommand<Unit, Unit> ExportCsv { get; }

    /// <summary>
    /// Gets the command that initiates retrieval of the list of tables from the data source.
    /// </summary>
    /// <remarks>The command can be executed to trigger the process of listing available tables. The command's
    /// execution and availability may be bound to UI elements in reactive user interfaces. This property is typically
    /// used in MVVM scenarios to encapsulate the logic for listing tables.</remarks>
    public ReactiveCommand<Unit, Unit> ListTables { get; }

    /// <summary>
    /// Gets the command that creates the users table in the database asynchronously.
    /// </summary>
    /// <remarks>This command can be executed to ensure that the users table exists before performing
    /// operations that depend on it. The command completes when the table creation operation finishes. If the table
    /// already exists, executing the command has no effect.</remarks>
    public ReactiveCommand<Unit, Unit> CreateUsersTable { get; }

    /// <summary>
    /// Gets a command that drops the users table from the database.
    /// </summary>
    /// <remarks>Executing this command will permanently remove all user data from the database. Use with
    /// caution, as this action cannot be undone.</remarks>
    public ReactiveCommand<Unit, Unit> DropUsersTable { get; }

    /// <summary>
    /// Gets the command that executes the sample selection action.
    /// </summary>
    /// <remarks>Use this command to trigger the selection logic in response to user interaction or other
    /// events. The command completes immediately and does not return a result value.</remarks>
    public ReactiveCommand<Unit, Unit> SampleSelect { get; }

    /// <summary>
    /// Gets a command that inserts a sample item when executed.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SampleInsert { get; }

    /// <summary>
    /// Gets the command that deletes the currently selected sample.
    /// </summary>
    /// <remarks>This command can be used to trigger the deletion of a sample in the user interface. The
    /// command's ability to execute may depend on whether a sample is currently selected. Use the command's CanExecute
    /// observable to determine if deletion is currently possible.</remarks>
    public ReactiveCommand<Unit, Unit> SampleDelete { get; }
}
