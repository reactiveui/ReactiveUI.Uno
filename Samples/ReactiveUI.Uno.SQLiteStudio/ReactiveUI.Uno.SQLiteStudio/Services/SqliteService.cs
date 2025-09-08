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

        // Ensure folder exists
        var dir = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var flags = SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache;
        _connection = new SQLiteAsyncConnection(dbPath, flags);

        // Some platforms/providers will throw for this PRAGMA. Ignore failures.
        try
        {
            await _connection.ExecuteAsync("PRAGMA journal_mode=WAL;").ConfigureAwait(false);
        }
        catch (SQLiteException)
        {
            // ignore
        }
        catch
        {
            // ignore
        }

        // Enable FK constraints where supported (ignore failures)
        try
        {
            await _connection.ExecuteAsync("PRAGMA foreign_keys=ON;").ConfigureAwait(false);
        }
        catch
        {
            // ignore
        }
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
        if (string.IsNullOrWhiteSpace(sql))
        {
            return Array.Empty<object>();
        }

        var lowered = sql.Trim().ToLowerInvariant();
        if (lowered.StartsWith("select"))
        {
            if (lowered.Contains(" from users"))
            {
                var rows = await conn.QueryAsync<UserRow>(sql).ConfigureAwait(false);
                return rows.Cast<object>().ToList();
            }
            if (lowered.Contains("from sqlite_master"))
            {
                var rows = await conn.QueryAsync<MasterRow>(sql).ConfigureAwait(false);
                // Render as string list
                return rows.Select(r => (object)r.name).ToList();
            }

            // Fallback: try to get first column as strings
            try
            {
                var scalarList = await conn.QueryScalarsAsync<string>(sql).ConfigureAwait(false);
                return scalarList.Cast<object>().ToList();
            }
            catch
            {
                // Last resort: execute without mapping
                return new List<object> { "Query executed." };
            }
        }
        else
        {
            // Non-SELECT: execute and return status
            try
            {
                var affected = await conn.ExecuteAsync(sql).ConfigureAwait(false);
                return new List<object> { $"Rows affected: {affected}" };
            }
            catch (Exception ex)
            {
                return new List<object> { ex.Message };
            }
        }
    }

    private sealed class MasterRow
    {
        public string name { get; set; } = string.Empty;
    }

    private sealed class UserRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public override string ToString() => $"{Id}: {Name} ({Age})";
    }
}
