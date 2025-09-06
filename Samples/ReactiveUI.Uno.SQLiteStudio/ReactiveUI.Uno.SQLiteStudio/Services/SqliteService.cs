using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SQLite;

namespace ReactiveUI.Uno.SQLiteStudio.Services;

public sealed class SqliteService : ISqliteService
{
    private static readonly Lazy<ISqliteService> LazyInstance = new(() => new SqliteService());
    private SQLiteAsyncConnection? _connection;

    private SqliteService()
    {
    }

    public static ISqliteService Instance => LazyInstance.Value;

    public async Task InitializeAsync(string? dbPath = null)
    {
        if (_connection is not null)
        {
            return;
        }

        dbPath ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "app.db3");
        _connection = new SQLiteAsyncConnection(dbPath);
        await _connection.ExecuteScalarAsync<string>("PRAGMA journal_mode=WAL").ConfigureAwait(false);
    }

    public async Task EnsureSampleDataAsync()
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        await conn.ExecuteAsync("CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Age INTEGER);").ConfigureAwait(false);

        var count = await conn.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Users;").ConfigureAwait(false);
        if (count == 0)
        {
            await conn.ExecuteAsync("INSERT INTO Users (Name, Age) VALUES ('Alice', 30);").ConfigureAwait(false);
            await conn.ExecuteAsync("INSERT INTO Users (Name, Age) VALUES ('Bob', 42);").ConfigureAwait(false);
            await conn.ExecuteAsync("INSERT INTO Users (Name, Age) VALUES ('Charlie', 25);").ConfigureAwait(false);
        }
    }

    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection is null)
        {
            await InitializeAsync().ConfigureAwait(false);
        }

        return _connection!;
    }

    public async Task<IReadOnlyList<string>> ListTablesAsync()
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        var tables = await conn.QueryAsync<MasterRow>("SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY 1").ConfigureAwait(false);
        return tables.Select(x => x.name).Where(x => !string.IsNullOrWhiteSpace(x)).ToList()!;
    }

    public async Task ExecuteAsync(string sql)
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        await conn.ExecuteAsync(sql).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<object>> QueryAsync(string sql)
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        var rows = await conn.QueryAsync<dynamic>(sql).ConfigureAwait(false);
        return rows.Cast<object>().ToList();
    }

    private sealed class MasterRow
    {
        public string name { get; set; } = string.Empty;
    }
}
