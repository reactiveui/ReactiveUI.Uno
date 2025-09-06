namespace ReactiveUI.Uno.SQLiteStudio.Services;

public interface ISqliteService
{
    Task InitializeAsync(string? dbPath = null);
    Task EnsureSampleDataAsync();
    Task<IReadOnlyList<string>> ListTablesAsync();
    Task ExecuteAsync(string sql);
    Task<IReadOnlyList<object>> QueryAsync(string sql);
}
