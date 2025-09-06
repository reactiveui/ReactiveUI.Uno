using System.Collections.ObjectModel;
using ReactiveUI.Uno.SQLiteStudio.Services;

namespace ReactiveUI.Uno.SQLiteStudio.Presentation;

public class MainViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly ISqliteService _db;
    private readonly ICsvExportService _csv;

    private string? _queryText = "SELECT * FROM Users;";
    private ReadOnlyObservableCollection<object>? _results;
    private ObservableCollection<object> _resultsBacking = new();
    private string? _status;

    public MainViewModel(IScreen hostScreen, ISqliteService? sqlite = null, ICsvExportService? csv = null)
    {
        HostScreen = hostScreen;
        UrlPathSegment = "Main";

        _db = sqlite ?? Locator.Current.GetService<ISqliteService>()!;
        _csv = csv ?? Locator.Current.GetService<ICsvExportService>()!;

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

        ExportCsv = ReactiveCommand.CreateFromTask(async () =>
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
        }, this.WhenAnyValue(x => x.Results).Select(r => r is { Count: > 0 }));

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

    public string? UrlPathSegment { get; }

    public IScreen HostScreen { get; }

    public string? QueryText
    {
        get => _queryText;
        set => this.RaiseAndSetIfChanged(ref _queryText, value);
    }

    public ReadOnlyObservableCollection<object>? Results => _results;

    public string? Status
    {
        get => _status;
        set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    public ReactiveCommand<Unit, Unit> ExecuteQuery { get; }

    public ReactiveCommand<Unit, Unit> ExportCsv { get; }

    public ReactiveCommand<Unit, Unit> ListTables { get; }

    public ReactiveCommand<Unit, Unit> CreateUsersTable { get; }

    public ReactiveCommand<Unit, Unit> DropUsersTable { get; }

    public ReactiveCommand<Unit, Unit> SampleSelect { get; }

    public ReactiveCommand<Unit, Unit> SampleInsert { get; }

    public ReactiveCommand<Unit, Unit> SampleDelete { get; }
}
